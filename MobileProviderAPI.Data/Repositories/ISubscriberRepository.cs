using MobileProviderAPI.Models;

namespace MobileProviderAPI.Data.Repositories;

public interface ISubscriberRepository
{
    Task<Subscriber?> GetBySubscriberNoAsync(string subscriberNo);
    Task<Subscriber?> AddSubscriberAsync(Subscriber subscriber);
    Task<bool> SubscriberExistsAsync(string subscriberNo);
}

