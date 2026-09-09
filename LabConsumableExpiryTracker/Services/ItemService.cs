using AutoMapper;
using LabConsumableExpiryTracker.Commons.Result;
using LabConsumableExpiryTracker.Models;
using LabConsumableExpiryTracker.DTOs;
using LabConsumableExpiryTracker.Repositories.Interfaces;
using LabConsumableExpiryTracker.Services.Interfaces;

namespace LabConsumableExpiryTracker.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;
        private readonly ILotRepository _lotRepository;
        private readonly IMapper _mapper;
        private readonly TimeProvider _timeProvider;
        public ItemService(
            IItemRepository itemRepository, 
            IMapper mapper, 
            TimeProvider timeProvider,
            ILotRepository lotRepository)
        {
            _itemRepository = itemRepository;
            _mapper = mapper;
            _timeProvider = timeProvider;
            _lotRepository = lotRepository;
        }
        public async Task<ServiceResult<IEnumerable<ItemDto>>> GetAllItem(CancellationToken ct)
        {
            var items = await _itemRepository.GetAllWithLotsAsync(ct);
            var today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
            
            var itemIds = items.Select(item => item.Id).ToArray();
            var usableQuantities = await _lotRepository.GetUsableQuantityByItemIdsAsync(itemIds,today,ct);
            
            IEnumerable<ItemDto> data = items.Select(item =>
            {
                var totalUsableQuantity = usableQuantities.TryGetValue(item.Id,
                out var quantity)
                ? quantity
                : 0m;
                
                var dto = _mapper.Map<ItemDto>(item);
                dto.TotalRemainingQuantity = totalUsableQuantity;
                dto.StockStatus = item.GetStockStatus(totalUsableQuantity);
                return dto;
                }).ToList();


            return ServiceResult<IEnumerable<ItemDto>>.SuccessResult(
                data,
                "Items retrieved successfully.");
        }

        public async Task<ServiceResult<ItemDto>> GetByIdItem(Guid id, CancellationToken ct)
        {
            var item = await _itemRepository.GetByIdWithLotsAsync(id, ct);
            if (item is null)
            {
                return ServiceResult<ItemDto>.ErrorResult(
                    "Item not found.");
            }
            var today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
            var totalUsableQuantity = await _lotRepository.GetTotalUsableQuantityAsync(
                item.Id,
                today,
                ct);
            var response = _mapper.Map<ItemDto>(item);
            response.TotalRemainingQuantity = totalUsableQuantity;
            response.StockStatus = item.GetStockStatus(totalUsableQuantity);

            return ServiceResult<ItemDto>.SuccessResult(
                response,
                "Item retrieved successfully.");
        }

        public async Task<ServiceResult<ItemDto>> CreateItem(CreateItemDto dto, CancellationToken ct)
        {
            var existing = await _itemRepository.GetByCodeAsync(dto.Code, ct);
            if (existing is not null)
            {
                return ServiceResult<ItemDto>.ErrorResult("Item code already exists.");
            }

            var item = _mapper.Map<Item>(dto);
            var created = await _itemRepository.AddAsync(item, ct);
            var response = _mapper.Map<ItemDto>(created);

            return ServiceResult<ItemDto>.SuccessResult(
                response,
                "Item created successfully.");
        }

        public async Task<ServiceResult<ItemDto>> UpdateItem(
            Guid id,
            UpdateItemDto request,
            CancellationToken ct)
        {
            var item = await _itemRepository.GetByIdAsync(id, ct);

            if (item is null)
            {
                return ServiceResult<ItemDto>.ErrorResult(
                    "Item not found.");
            }
            var now = _timeProvider.GetUtcNow();

            var totalUsableQuantity = item.Lots
                .Where(lot => lot.IsEligible(now))
                .Sum(lot => lot.RemainingQuantity);

            var stockStatus = item.GetStockStatus(
                totalUsableQuantity);

            item.UpdateDetails(
                request.Name,
                request.BaseUnit,
                request.MinimumStock,
                request.ExpiringSoonDays);

            var updated = await _itemRepository.UpdateAsync(item, ct);
            var response = _mapper.Map<ItemDto>(updated);

            return ServiceResult<ItemDto>.SuccessResult(
                response,
                "Item updated successfully.");
        }

        public async Task<ServiceResult<bool>> DeleteItem(
            Guid id,
            CancellationToken ct)
        {
            var deleted = await _itemRepository.DeleteAsync(id, ct);

            return deleted
                ? ServiceResult<bool>.SuccessResult(
                    true,
                    "Item deleted successfully.")
                : ServiceResult<bool>.ErrorResult(
                    "Item not found.");
        }
    }
}