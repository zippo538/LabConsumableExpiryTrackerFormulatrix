using AutoMapper;
using LabConsumableExpireTracker.Models;
using LabConsumableExpiryTracker.DTOs;
using LabConsumableExpiryTracker.Repositories;
using LabConsumableExpiryTracker.Services.Interfaces;

namespace LabConsumableExpiryTracker.Services
{
    public class LotServices : ILotService
    {
        private readonly ILotRepository _lotRepository;
        private readonly IMapper _mapper;

        public LotServices(ILotRepository lotRepository, IMapper mapper)
        {
            _lotRepository = lotRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponseDTO<IEnumerable<LotDTO>>> GetAll(CancellationToken ct)
        {
            var lots = await _lotRepository.GetAllAsync(ct);
            var response = _mapper.Map<IEnumerable<LotDTO>>(lots);
            return ApiResponseDTO<LotDTO>.SuccessResult(response, "Lots retrieved successfully.");
        }

        public async Task<ApiResponseDTO<IEnumerable<LotDTO>>> GetByItemId(Guid itemId, CancellationToken ct)
        {
            var lots = await _lotRepository.GetByItemIdAsync(itemId, ct);
            return ApiResponseDTO<LotDTO>.SuccessResult(_mapper.Map<IEnumerable<LotDTO>>(lots), "Lots retrieved successfully.");
        }

        public async Task<ApiResponseDTO<LotDTO>> GetById(Guid id, CancellationToken ct)
        {

            var lot = await _lotRepository.GetByIdAsync(id, ct);
            if (lot is null)
            {
                return ApiResponseDTO<LotDTO>.ErrorResult("NotFound");
            }
            var response = _mapper.Map<LotDTO>(lot);
            return ApiResponseDTO<LotDTO>.SuccessResult(response);
        }

        public async Task<ApiResponseDTO<LotDTO>> Create(CreateLotDTO createLotDTO, CancellationToken ct)
        {
            var lot = _mapper.Map<Lot>(createLotDTO);
            var created = await _lotRepository.AddAsync(lot, ct);
            var response = _mapper.Map<LotDTO>(created);

            return ApiResponseDTO<LotDTO>.SuccessResult(response, "Lot created successfully.");
        }

        public async Task<ApiResponseDTO<LotDTO>> Update(Guid id, UpdateLotDTO updateLotDTO, CancellationToken ct)
        {
            var existing = await _lotRepository.GetByIdAsync(id, ct);
            if (existing is null)
            {
                return ApiResponseDTO<LotDTO>.ErrorResult("Lot not found.");
            }
            _mapper.Map(updateLotDTO, existing);
            var updated = await _lotRepository.UpdateAsync(existing, ct);
            var response = _mapper.Map<LotDTO>(updated);

            return ApiResponseDTO<LotDTO>.SuccessResult(response, "Lot updated successfully.");
        }

        public async Task<ApiResponseDTO<bool>> Delete(Guid id, CancellationToken ct)
        {
            var deleted = await _lotRepository.DeleteAsync(id, ct);
            return deleted
            ? ApiResponseDTO<bool>.SuccessResult(
                true,
                "Lot deleted successfully.")
            : ApiResponseDTO<bool>.ErrorResult(
                "Lot not found.");
        }

    }
}