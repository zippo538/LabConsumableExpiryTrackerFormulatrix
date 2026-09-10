using LabConsumableExpiryTracker.Commons.Result;
using LabConsumableExpiryTracker.DTOs;

namespace LabConsumableExpiryTracker.Services.Interfaces
{
    public interface ILotService
    {
        Task<ServiceResult<IEnumerable<LotDto>>> GetAllLot(CancellationToken ct);

        Task<ServiceResult<IEnumerable<LotSummaryDto>>> GetSummaryByItemId(Guid itemId, CancellationToken ct);
        Task<ServiceResult<IEnumerable<LotSummaryDto>>> GetAllSummary(CancellationToken ct);
        Task<ServiceResult<LotDto>> GetByIdLot(Guid id, CancellationToken ct);
        Task<ServiceResult<LotDto>> CreateLot(CreateLotDto lot, CancellationToken ct);
        Task<ServiceResult<LotDto>> UpdateLot(Guid id, UpdateLotDto lot, CancellationToken ct);
        Task<ServiceResult<bool>> DeleteLot(Guid id, CancellationToken ct);
    }
}