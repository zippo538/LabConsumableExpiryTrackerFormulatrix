using LabConsumableExpiryTracker.Models;

namespace LabConsumableExpiryTracker.Repositories.Interfaces;

public interface ILotRepository : IRepository<Lot, Guid>
{
    Task<IEnumerable<Lot>> GetByItemIdAsync(Guid itemId, CancellationToken ct = default);
}
