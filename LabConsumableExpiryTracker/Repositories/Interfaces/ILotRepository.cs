using LabConsumableExpireTracker.Models;

namespace LabConsumableExpiryTracker.Repositories;

public interface ILotRepository
{
    Task<IEnumerable<Lot>> GetAllAsync(CancellationToken ct = default);
    Task<Lot?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Lot>> GetByItemIdAsync(Guid itemId, CancellationToken ct = default);
    Task<Lot> AddAsync(Lot lot, CancellationToken ct = default);
    Task<Lot> UpdateAsync(Lot lot, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
