using System.Collections.Concurrent;
using LabConsumableExpireTracker.Models;
using LabConsumableExpiryTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace LabConsumableExpiryTracker.Repositories;

public class LotRepository : ILotRepository
{
    private readonly AppDBContext _context;

    public LotRepository(AppDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Lot>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Lots
            .AsNoTracking()
            .OrderByDescending(lot => lot.ReceivedAt)
            .ToListAsync(ct);
    }

    public async Task<Lot?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Lots
            .FirstOrDefaultAsync(lot => lot.Id == id, ct);
    }

    public async Task<IEnumerable<Lot>> GetByItemIdAsync(
        Guid itemId,
        CancellationToken ct = default)
    {
        return await _context.Lots
            .AsNoTracking()
            .Where(lot => lot.ItemId == itemId)
            .OrderByDescending(lot => lot.ReceivedAt)
            .ToListAsync(ct);
    }

    public async Task<Lot> AddAsync(Lot lot, CancellationToken ct = default)
    {
        await _context.Lots.AddAsync(lot, ct);
        await _context.SaveChangesAsync(ct);

        return lot;
    }

    public async Task<Lot> UpdateAsync(Lot lot, CancellationToken ct = default)
    {
        _context.Lots.Update(lot);
        await _context.SaveChangesAsync(ct);

        return lot;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var lot = await _context.Lots
            .FirstOrDefaultAsync(item => item.Id == id, ct);

        if (lot is null)
        {
            return false;
        }

        _context.Lots.Remove(lot);
        await _context.SaveChangesAsync(ct);

        return true;
    }
}
