using Microsoft.EntityFrameworkCore;
using MobileProviderAPI.Data.Repositories;
using MobileProviderAPI.Models;

namespace MobileProviderAPI.Data.Repositories;

public class SubscriberRepository : ISubscriberRepository
{
    private readonly ApplicationDbContext _context;

    public SubscriberRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Subscriber?> GetBySubscriberNoAsync(string subscriberNo)
    {
        return await _context.Subscribers
            .FirstOrDefaultAsync(s => s.SubscriberNo == subscriberNo);
    }

    public async Task<Subscriber?> AddSubscriberAsync(Subscriber subscriber)
    {
        await _context.Subscribers.AddAsync(subscriber);
        await _context.SaveChangesAsync();
        return subscriber;
    }

    public async Task<bool> SubscriberExistsAsync(string subscriberNo)
    {
        return await _context.Subscribers
            .AnyAsync(s => s.SubscriberNo == subscriberNo);
    }
}

