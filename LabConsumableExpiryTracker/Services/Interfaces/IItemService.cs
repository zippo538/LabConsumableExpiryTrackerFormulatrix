using LabConsumableExpiryTracker.DTOs;

namespace LabConsumableExpiryTracker.Services.Interfaces
{
    public interface IItemService
    {
        Task<ApiResponseDTO<IEnumerable<ItemDTO>>> GetAllItem(
       CancellationToken ct);

        Task<ApiResponseDTO<ItemDTO>> GetByIdItem(
            Guid id,
            CancellationToken ct);

        Task<ApiResponseDTO<ItemDTO>> CreateItem(
            CreateItemDTO lot,
            CancellationToken ct);

        Task<ApiResponseDTO<ItemDTO>> UpdateItem(
            Guid id,
            UpdateItemDTO lot,
            CancellationToken ct);

        Task<ApiResponseDTO<bool>> DeleteItem(
            Guid id,
            CancellationToken ct);
    }
    }
