using LabConsumableExpiryTracker.Commons.Result;
using LabConsumableExpiryTracker.DTOs;

namespace LabConsumableExpiryTracker.Services.Interfaces
{
    public interface IItemService
    {
        Task<ServiceResult<IEnumerable<ItemDTO>>> GetAllItem(
       CancellationToken ct);

        Task<ServiceResult<ItemDTO>> GetByIdItem(
            Guid id,
            CancellationToken ct);

        Task<ServiceResult<ItemDTO>> CreateItem(
            CreateItemDTO lot,
            CancellationToken ct);

        Task<ServiceResult<ItemDTO>> UpdateItem(
            Guid id,
            UpdateItemDTO lot,
            CancellationToken ct);

        Task<ServiceResult<bool>> DeleteItem(
            Guid id,
            CancellationToken ct);
    }
    }
