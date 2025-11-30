using Microsoft.EntityFrameworkCore;
using MobileProviderAPI.Data.Repositories;
using MobileProviderAPI.Models;

namespace MobileProviderAPI.Data.Repositories;

public class BillRepository : IBillRepository
{
    private readonly ApplicationDbContext _context;

    public BillRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Bill?> GetBillBySubscriberAndMonthAsync(string subscriberNo, string month)
    {
        return await _context.Bills
            .Include(b => b.BillDetails)
            .Include(b => b.Subscriber)
            .FirstOrDefaultAsync(b => b.SubscriberNo == subscriberNo && b.Month == month);
    }

    public async Task<List<Bill>> GetUnpaidBillsBySubscriberAsync(string subscriberNo)
    {
        return await _context.Bills
            .Where(b => b.SubscriberNo == subscriberNo && !b.IsPaid)
            .OrderBy(b => b.Month)
            .ToListAsync();
    }

    public async Task<Bill?> AddBillAsync(Bill bill)
    {
        await _context.Bills.AddAsync(bill);
        await _context.SaveChangesAsync();
        return bill;
    }

    public async Task<bool> UpdateBillAsync(Bill bill)
    {
        _context.Bills.Update(bill);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> BillExistsAsync(string subscriberNo, string month)
    {
        return await _context.Bills
            .AnyAsync(b => b.SubscriberNo == subscriberNo && b.Month == month);
    }

    public async Task<List<Bill>> GetBillsBySubscriberAsync(string subscriberNo)
    {
        return await _context.Bills
            .Where(b => b.SubscriberNo == subscriberNo)
            .OrderByDescending(b => b.Month)
            .ToListAsync();
    }
}

