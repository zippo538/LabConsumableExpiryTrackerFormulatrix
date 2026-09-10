using LabConsumableExpiryTracker.Models;

namespace LabConsumableExpiryTracker.Repositories.Interfaces
{
    public interface IJobRepository : IRepository<Job, Guid>
    {
        Task<Job?> GetByJobNumberAsync(string jobNumber, CancellationToken ct = default);
        Task<bool> ExistsByJobNumberAsync(string jobNumber, CancellationToken ct = default);
    }
}