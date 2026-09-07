using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LabConsumableExpiryTracker.Commons.Result;
using LabConsumableExpiryTracker.Models;
using LabConsumableExpiryTracker.DTOs;

namespace LabConsumableExpiryTracker.Services.Interfaces
{
    public interface ILotService
    {
        Task<ServiceResult<IEnumerable<LotDTO>>> GetAllLot(CancellationToken ct);

        Task<ServiceResult<IEnumerable<LotSummaryDTO>>> GetSummaryByItemId(Guid itemId, CancellationToken ct);
        Task<ServiceResult<IEnumerable<LotSummaryDTO>>> GetAllSummary(CancellationToken ct);
        Task<ServiceResult<LotDTO>> GetByIdLot(Guid id, CancellationToken ct);
        Task<ServiceResult<LotDTO>> CreateLot(CreateLotDTO lot, CancellationToken ct);
        Task<ServiceResult<LotDTO>> UpdateLot(Guid id, UpdateLotDTO lot, CancellationToken ct);
        Task<ServiceResult<bool>> DeleteLot(Guid id, CancellationToken ct);
    }
}