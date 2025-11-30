using Microsoft.EntityFrameworkCore;
using MobileProviderAPI.Data.Repositories;
using MobileProviderAPI.Models;

namespace MobileProviderAPI.Data.Repositories;

public class ApiCallLogRepository : IApiCallLogRepository
{
    private readonly ApplicationDbContext _context;

    public ApiCallLogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task LogApiCallAsync(ApiCallLog log)
    {
        await _context.ApiCallLogs.AddAsync(log);
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetApiCallCountTodayAsync(string subscriberNo, string endpoint)
    {
        var today = DateTime.UtcNow.Date;
        return await _context.ApiCallLogs
            .CountAsync(l => l.SubscriberNo == subscriberNo 
                && l.Endpoint == endpoint 
                && l.CallDate.Date == today);
    }
}

