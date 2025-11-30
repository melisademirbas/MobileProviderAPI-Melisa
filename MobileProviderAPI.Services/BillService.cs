using MobileProviderAPI.Data.Repositories;
using MobileProviderAPI.Models;
using MobileProviderAPI.Models.DTOs;

namespace MobileProviderAPI.Services;

public class BillService : IBillService
{
    private readonly IBillRepository _billRepository;
    private readonly ISubscriberRepository _subscriberRepository;
    private readonly IPaymentRepository _paymentRepository;

    public BillService(
        IBillRepository billRepository,
        ISubscriberRepository subscriberRepository,
        IPaymentRepository paymentRepository)
    {
        _billRepository = billRepository;
        _subscriberRepository = subscriberRepository;
        _paymentRepository = paymentRepository;
    }

    public async Task<QueryBillResponse?> QueryBillAsync(string subscriberNo, string month)
    {
        var bill = await _billRepository.GetBillBySubscriberAndMonthAsync(subscriberNo, month);
        if (bill == null)
            return null;

        return new QueryBillResponse
        {
            BillTotal = bill.TotalAmount,
            PaidStatus = bill.IsPaid,
            Month = bill.Month
        };
    }

    public async Task<QueryBillDetailedResponse?> QueryBillDetailedAsync(string subscriberNo, string month, int pageNumber = 1, int pageSize = 10)
    {
        var bill = await _billRepository.GetBillBySubscriberAndMonthAsync(subscriberNo, month);
        if (bill == null)
            return null;

        var billDetails = bill.BillDetails
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(bd => new BillDetailDto
            {
                Description = bd.Description,
                Amount = bd.Amount,
                Category = bd.Category
            })
            .ToList();

        return new QueryBillDetailedResponse
        {
            BillTotal = bill.TotalAmount,
            Month = bill.Month,
            BillDetails = billDetails
        };
    }

    public async Task<List<QueryBillResponse>> QueryUnpaidBillsAsync(string subscriberNo)
    {
        var bills = await _billRepository.GetUnpaidBillsBySubscriberAsync(subscriberNo);
        return bills.Select(b => new QueryBillResponse
        {
            BillTotal = b.TotalAmount,
            PaidStatus = b.IsPaid,
            Month = b.Month
        }).ToList();
    }

    public async Task<PayBillResponse> PayBillAsync(string subscriberNo, string month, decimal? amount)
    {
        var bill = await _billRepository.GetBillBySubscriberAndMonthAsync(subscriberNo, month);
        if (bill == null)
        {
            return new PayBillResponse
            {
                PaymentStatus = "Error",
                ErrorMessage = "Bill not found",
                PaidAmount = 0,
                RemainingAmount = 0
            };
        }

        var paymentAmount = amount ?? (bill.TotalAmount - bill.PaidAmount);
        var newPaidAmount = bill.PaidAmount + paymentAmount;

        // If payment exceeds total, only pay the remaining amount
        if (newPaidAmount > bill.TotalAmount)
        {
            paymentAmount = bill.TotalAmount - bill.PaidAmount;
            newPaidAmount = bill.TotalAmount;
        }

        bill.PaidAmount = newPaidAmount;

        var subscriber = await _subscriberRepository.GetBySubscriberNoAsync(subscriberNo);
        if (subscriber == null)
        {
            return new PayBillResponse
            {
                PaymentStatus = "Error",
                ErrorMessage = "Subscriber not found",
                PaidAmount = 0,
                RemainingAmount = bill.TotalAmount - bill.PaidAmount
            };
        }

        var payment = new Payment
        {
            BillId = bill.Id,
            SubscriberId = subscriber.Id,
            SubscriberNo = subscriberNo,
            Amount = paymentAmount,
            Status = "Successful",
            PaymentDate = DateTime.UtcNow
        };

        await _paymentRepository.AddPaymentAsync(payment);
        await _billRepository.UpdateBillAsync(bill);

        return new PayBillResponse
        {
            PaymentStatus = "Successful",
            PaidAmount = paymentAmount,
            RemainingAmount = bill.TotalAmount - bill.PaidAmount
        };
    }

    public async Task<TransactionResponse> AddBillAsync(AddBillRequest request)
    {
        try
        {
            var subscriber = await _subscriberRepository.GetBySubscriberNoAsync(request.SubscriberNo);
            if (subscriber == null)
            {
                subscriber = new Subscriber
                {
                    SubscriberNo = request.SubscriberNo,
                    Name = $"Subscriber {request.SubscriberNo}",
                    Email = $"{request.SubscriberNo}@example.com",
                    CreatedAt = DateTime.UtcNow
                };
                subscriber = await _subscriberRepository.AddSubscriberAsync(subscriber);
            }

            var billExists = await _billRepository.BillExistsAsync(request.SubscriberNo, request.Month);
            if (billExists)
            {
                return new TransactionResponse
                {
                    Status = "Error",
                    ErrorMessage = $"Bill for subscriber {request.SubscriberNo} and month {request.Month} already exists"
                };
            }

            var bill = new Bill
            {
                SubscriberId = subscriber.Id,
                SubscriberNo = request.SubscriberNo,
                Month = request.Month,
                TotalAmount = request.TotalAmount,
                PaidAmount = 0,
                CreatedAt = DateTime.UtcNow
            };

            if (request.BillDetails != null && request.BillDetails.Any())
            {
                bill.BillDetails = request.BillDetails.Select(bd => new BillDetail
                {
                    Description = bd.Description,
                    Amount = bd.Amount,
                    Category = bd.Category,
                    CreatedAt = DateTime.UtcNow
                }).ToList();
            }

            await _billRepository.AddBillAsync(bill);

            return new TransactionResponse
            {
                Status = "Success",
                RecordsProcessed = 1
            };
        }
        catch (Exception ex)
        {
            return new TransactionResponse
            {
                Status = "Error",
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<TransactionResponse> AddBillsBatchAsync(Stream csvStream)
    {
        var recordsProcessed = 0;
        var errors = new List<string>();

        try
        {
            using var reader = new StreamReader(csvStream);
            var lineNumber = 0;

            while (!reader.EndOfStream)
            {
                lineNumber++;
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line) || lineNumber == 1) // Skip header
                    continue;

                var parts = line.Split(',');
                if (parts.Length < 2)
                {
                    errors.Add($"Line {lineNumber}: Invalid format");
                    continue;
                }

                var subscriberNo = parts[0].Trim();
                var month = parts[1].Trim();
                var totalAmount = parts.Length > 2 && decimal.TryParse(parts[2].Trim(), out var amount) 
                    ? amount 
                    : 100.00m; // Default amount

                var request = new AddBillRequest
                {
                    SubscriberNo = subscriberNo,
                    Month = month,
                    TotalAmount = totalAmount
                };

                var result = await AddBillAsync(request);
                if (result.Status == "Success")
                {
                    recordsProcessed++;
                }
                else
                {
                    errors.Add($"Line {lineNumber}: {result.ErrorMessage}");
                }
            }

            return new TransactionResponse
            {
                Status = errors.Any() ? "Partial Success" : "Success",
                RecordsProcessed = recordsProcessed,
                ErrorMessage = errors.Any() ? string.Join("; ", errors.Take(10)) : null
            };
        }
        catch (Exception ex)
        {
            return new TransactionResponse
            {
                Status = "Error",
                ErrorMessage = ex.Message,
                RecordsProcessed = recordsProcessed
            };
        }
    }
}

