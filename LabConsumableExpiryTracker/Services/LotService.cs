using AutoMapper;
using LabConsumableExpiryTracker.Commons.Result;
using LabConsumableExpiryTracker.Models;
using LabConsumableExpiryTracker.DTOs;
using LabConsumableExpiryTracker.Repositories.Interfaces;
using LabConsumableExpiryTracker.Services.Interfaces;

namespace LabConsumableExpiryTracker.Services
{
    public class LotService : ILotService
    {
        private readonly ILotRepository _lotRepository;
        private readonly IMapper _mapper;

        public LotService(ILotRepository lotRepository, IMapper mapper)
        {
            _lotRepository = lotRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResult<IEnumerable<LotDTO>>> GetAllLot(CancellationToken ct)
        {
            var lots = await _lotRepository.GetAllAsync(ct);
            var response = _mapper.Map<IEnumerable<LotDTO>>(lots);
            return ServiceResult<LotDTO>.SuccessResult(
                response,
                "Lots retrieved successfully.");
        }



        public async Task<ServiceResult<LotDTO>> GetByIdLot(Guid id, CancellationToken ct)
        {

            var lot = await _lotRepository.GetByIdAsync(id, ct);
            if (lot is null)
            {
                return ServiceResult<LotDTO>.ErrorResult("NotFound");
            }
            var response = _mapper.Map<LotDTO>(lot);
            return ServiceResult<LotDTO>.SuccessResult(response);
        }

        public async Task<ServiceResult<LotDTO>> CreateLot(CreateLotDTO createLotDTO, CancellationToken ct)
        {
            var lot = _mapper.Map<Lot>(createLotDTO);
            var created = await _lotRepository.AddAsync(lot, ct);
            var response = _mapper.Map<LotDTO>(created);

            return ServiceResult<LotDTO>.SuccessResult(
                response,
                "Lot created successfully.");
        }

        public async Task<ServiceResult<LotDTO>> UpdateLot(Guid id, UpdateLotDTO updateLotDTO, CancellationToken ct)
        {
            var existing = await _lotRepository.GetByIdAsync(id, ct);
            if (existing is null)
            {
                return ServiceResult<LotDTO>.ErrorResult("Lot not found.");
            }
            existing.UpdateDetails(
                updateLotDTO.RemainingQuantity,
                updateLotDTO.StorageLocation);

            var updated = await _lotRepository.UpdateAsync(existing, ct);
            var response = _mapper.Map<LotDTO>(updated);
            return ServiceResult<LotDTO>.SuccessResult(
                response,
                "Lot updated successfully.");
        }

        public async Task<ServiceResult<bool>> DeleteLot(Guid id, CancellationToken ct)
        {
            var deleted = await _lotRepository.DeleteAsync(id, ct);
            return deleted
            ? ServiceResult<bool>.SuccessResult(
                true,
                "Lot deleted successfully.")
            : ServiceResult<bool>.ErrorResult(
                "Lot not found.");
        }
        // Lot Summary 
        public async Task<ServiceResult<IEnumerable<LotSummaryDTO>>> GetSummaryByItemId(Guid itemId, CancellationToken ct)
        {
            var lots = await _lotRepository.GetByItemIdAsync(itemId, ct);
            var summaries = _mapper.Map<IEnumerable<LotSummaryDTO>>(lots);
            return ServiceResult<LotSummaryDTO>.SuccessResult(
                summaries,
                "Lots retrieved successfully.");
        }

        public async Task<ServiceResult<IEnumerable<LotSummaryDTO>>> GetAllSummary(CancellationToken ct)
        {
            var lots = await _lotRepository.GetAllAsync(ct);

            var summaries = _mapper.Map<IEnumerable<LotSummaryDTO>>(lots);

            return ServiceResult<IEnumerable<LotSummaryDTO>>.SuccessResult(
            summaries,
            "Lot summaries retrieved successfully.");
        }
    }
}