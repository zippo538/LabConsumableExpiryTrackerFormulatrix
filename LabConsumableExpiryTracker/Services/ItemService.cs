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
        private readonly IMapper _mapper;
        public ItemService(IItemRepository itemRepository, IMapper mapper)
        {
            _itemRepository = itemRepository;
            _mapper = mapper;
        }
        public async Task<ServiceResult<IEnumerable<ItemDto>>> GetAllItem(CancellationToken ct)
        {
            var items = await _itemRepository.GetAllAsync(ct);
            var data = _mapper.Map<IEnumerable<ItemDto>>(items);

            return ServiceResult<IEnumerable<ItemDto>>.SuccessResult(
                data,
                "Items retrieved successfully.");
        }

        public async Task<ServiceResult<ItemDto>> GetByIdItem(Guid id, CancellationToken ct)
        {
            var item = await _itemRepository.GetByIdAsync(id, ct);

            if (item is null)
            {
                return ServiceResult<ItemDto>.ErrorResult(
                    "Item not found.");
            }
            var response = _mapper.Map<ItemDto>(item);

            return ServiceResult<ItemDto>.SuccessResult(
                response,
                "Item retrieved successfully.");
        }

        public async Task<ServiceResult<ItemDto>> CreateItem(CreateItemDto dto, CancellationToken ct)
        {
            var existing = await _itemRepository.GetByCodeAsync(
dto.Code,
                ct);

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
            UpdateItemDto dto,
            CancellationToken ct)
        {
            var item = await _itemRepository.GetByIdAsync(id, ct);

            if (item is null)
            {
                return ServiceResult<ItemDto>.ErrorResult(
                    "Item not found.");
            }

            item.UpdateDetails(
                dto.Name,
                dto.BaseUnit,
                dto.MinimumStock,
                dto.ExpiringSoonDays);

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