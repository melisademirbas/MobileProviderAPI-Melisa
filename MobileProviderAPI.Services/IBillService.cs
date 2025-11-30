using MobileProviderAPI.Models.DTOs;

namespace MobileProviderAPI.Services;

public interface IBillService
{
    Task<QueryBillResponse?> QueryBillAsync(string subscriberNo, string month);
    Task<QueryBillDetailedResponse?> QueryBillDetailedAsync(string subscriberNo, string month, int pageNumber = 1, int pageSize = 10);
    Task<List<QueryBillResponse>> QueryUnpaidBillsAsync(string subscriberNo);
    Task<PayBillResponse> PayBillAsync(string subscriberNo, string month, decimal? amount);
    Task<TransactionResponse> AddBillAsync(AddBillRequest request);
    Task<TransactionResponse> AddBillsBatchAsync(Stream csvStream);
}

