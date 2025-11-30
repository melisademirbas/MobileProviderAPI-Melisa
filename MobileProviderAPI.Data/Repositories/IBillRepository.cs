using MobileProviderAPI.Models;

namespace MobileProviderAPI.Data.Repositories;

public interface IBillRepository
{
    Task<Bill?> GetBillBySubscriberAndMonthAsync(string subscriberNo, string month);
    Task<List<Bill>> GetUnpaidBillsBySubscriberAsync(string subscriberNo);
    Task<Bill?> AddBillAsync(Bill bill);
    Task<bool> UpdateBillAsync(Bill bill);
    Task<bool> BillExistsAsync(string subscriberNo, string month);
    Task<List<Bill>> GetBillsBySubscriberAsync(string subscriberNo);
}

