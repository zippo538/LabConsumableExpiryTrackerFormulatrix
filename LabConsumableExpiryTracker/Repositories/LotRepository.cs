using LabConsumableExpiryTracker.Models;
using LabConsumableExpiryTracker.Data;
using LabConsumableExpiryTracker.Data.Seeders;
using LabConsumableExpiryTracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LabConsumableExpiryTracker.Repositories;

public class LotRepository : Repository<Lot, Guid>, ILotRepository
{
    private readonly AppDbContext _context;

    public LotRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(Guid itemId, string lotNumber, DateOnly expiryDate, CancellationToken ct)
    {
        var normalizedLotNumber = lotNumber.Trim();
        return await _context.Lots
        .AsNoTracking()
        .AnyAsync(
           lot =>
               lot.ItemId == itemId &&
               lot.LotNumber == normalizedLotNumber &&
               lot.ExpiryDate == expiryDate,
               ct
           );
    }

    public async Task<IEnumerable<Lot>> GetByItemIdAsync(
        Guid itemId,
        CancellationToken ct = default)
    {
        return await _context.Lots
            .AsNoTracking()
            .Where(lot => lot.ItemId == itemId)
            .OrderBy(lot => lot.ExpiryDate)
            .ThenBy(lot => lot.ReceivedAt)
            .ToListAsync(ct);
    }
}
