using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LabConsumableExpiryTracker.Models;
using LabConsumableExpiryTracker.DTOs;

namespace LabConsumableExpiryTracker.Services.Interfaces
{
    public interface ILotService
    {
        Task<ApiResponseDTO<IEnumerable<LotDTO>>> GetAllLot(CancellationToken ct);

        Task<ApiResponseDTO<IEnumerable<LotSummaryDTO>>> GetSummaryByItemId(Guid itemId, CancellationToken ct);
        Task<ApiResponseDTO<IEnumerable<LotSummaryDTO>>> GetAllSummary(CancellationToken ct);
        Task<ApiResponseDTO<LotDTO>> GetByIdLot(Guid id, CancellationToken ct);
        Task<ApiResponseDTO<LotDTO>> CreateLot(CreateLotDTO lot, CancellationToken ct);
        Task<ApiResponseDTO<LotDTO>> UpdateLot(Guid id, UpdateLotDTO lot, CancellationToken ct);
        Task<ApiResponseDTO<bool>> DeleteLot(Guid id, CancellationToken ct);
    }
}