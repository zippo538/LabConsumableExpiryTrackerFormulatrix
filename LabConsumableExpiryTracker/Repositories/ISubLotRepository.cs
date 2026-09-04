using LabConsumableExpireTracker.Domain.Entities;

namespace LabConsumableExpireTracker.Domain.Repositories;

public interface ISubLotRepository
{
    Task<IReadOnlyList<SubLot>> GetCandidatesAsync(
        Guid itemId,
        DateOnly today,
        CancellationToken cancellationToken = default);

    Task<SubLot?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        SubLot subLot,
        CancellationToken cancellationToken = default);
}
