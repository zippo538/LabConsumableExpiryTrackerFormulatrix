using System.Collections.Concurrent;
using LabConsumableExpireTracker.Models;

namespace LabConsumableExpiryTracker.Repositories;

public class LotRepository : ILotRepository
{
    private readonly ConcurrentDictionary<Guid, Lot> _lots = new();

    public Task<IEnumerable<Lot>> GetAllAsync(CancellationToken ct = default)
    {
        IEnumerable<Lot> result = _lots.Values
            .OrderByDescending(l => l.ReceivedAt)
            .ToList();
        return Task.FromResult(result);
    }

    public Task<Lot?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        _lots.TryGetValue(id, out var lot);
        return Task.FromResult(lot);
    }

    public Task<IEnumerable<Lot>> GetByItemIdAsync(Guid itemId, CancellationToken ct = default)
    {
        IEnumerable<Lot> result = _lots.Values
            .Where(l => l.ItemId == itemId)
            .OrderByDescending(l => l.ReceivedAt)
            .ToList();
        return Task.FromResult(result);
    }

    public Task<Lot> AddAsync(Lot lot, CancellationToken ct = default)
    {
        if (!_lots.TryAdd(lot.Id, lot))
            throw new InvalidOperationException($"Lot with ID '{lot.Id}' already exists.");

        return Task.FromResult(lot);
    }

    public Task<Lot> UpdateAsync(Lot lot, CancellationToken ct = default)
    {
        if (!_lots.ContainsKey(lot.Id))
            throw new KeyNotFoundException($"Lot with ID '{lot.Id}' not found.");

        _lots[lot.Id] = lot;
        return Task.FromResult(lot);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        return Task.FromResult(_lots.TryRemove(id, out _));
    }
}
