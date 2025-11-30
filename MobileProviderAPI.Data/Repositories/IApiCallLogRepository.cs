using MobileProviderAPI.Models;

namespace MobileProviderAPI.Data.Repositories;

public interface IApiCallLogRepository
{
    Task LogApiCallAsync(ApiCallLog log);
    Task<int> GetApiCallCountTodayAsync(string subscriberNo, string endpoint);
}

