using LabConsumableExpiryTracker.Models;
namespace LabConsumableExpiryTracker.Repositories.Interfaces
{
    public interface IItemRepository : IRepository<Item, Guid>
    {
        Task<Item?> GetByCodeAsync(
            string code,
            CancellationToken ct = default
        );
    }
}