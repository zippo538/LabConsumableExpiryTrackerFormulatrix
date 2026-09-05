using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LabConsumableExpireTracker.Models;
using LabConsumableExpiryTracker.DTOs;

namespace LabConsumableExpiryTracker.Services.Interfaces
{
    public interface ILotService
    {
        Task<ApiResponseDTO<IEnumerable<LotDTO>>> GetAll(
       CancellationToken ct);

        Task<ApiResponseDTO<IEnumerable<LotDTO>>> GetByItemId(
            Guid itemId,
            CancellationToken ct);

        Task<ApiResponseDTO<LotDTO>> GetById(
            Guid id,
            CancellationToken ct);

        Task<ApiResponseDTO<LotDTO>> Create(
            CreateLotDTO lot,
            CancellationToken ct);

        Task<ApiResponseDTO<LotDTO>> Update(
            Guid id,
            UpdateLotDTO lot,
            CancellationToken ct);

        Task<ApiResponseDTO<bool>> Delete(
            Guid id,
            CancellationToken ct);
    }
}