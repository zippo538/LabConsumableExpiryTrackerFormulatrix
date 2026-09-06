using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using LabConsumableExpiryTracker.DTOs;
using LabConsumableExpiryTracker.Services;
using LabConsumableExpiryTracker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LabConsumableExpiryTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly ILotService _lotService;
        private readonly IValidator<CreateItemDTO> _createItemValidator;
        private readonly IValidator<UpdateItemDTO> _updateItemValidator;


        public ItemController(
            IItemService itemService,
            IValidator<CreateItemDTO> createItemValidator,
            IValidator<UpdateItemDTO> updateItemValidator,
            ILotService lotService
            )
        {
            _itemService = itemService;
            _createItemValidator = createItemValidator;
            _updateItemValidator = updateItemValidator;
            _lotService = lotService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDTO<IEnumerable<ItemDTO>>>> GetAll(
            CancellationToken ct = default)
        {
            var response = await _itemService.GetAllItem(ct);
            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponseDTO<ItemDTO>>> GetById(
            Guid id,
            CancellationToken ct = default)
        {
            var response = await _itemService.GetByIdItem(id, ct);

            return response.Success
                ? Ok(response)
                : NotFound(response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDTO<ItemDTO>>> Create(
            [FromBody] CreateItemDTO dto,
            CancellationToken ct = default)
        {
            var validation = await _createItemValidator.ValidateAsync(dto, ct);

            if (!validation.IsValid)
            {
                return BadRequest(ApiResponseDTO<ItemDTO>.ErrorResult(
                    "Validation failed.",
                    validation.Errors
                        .Select(error => error.ErrorMessage)
                        .ToList()));
            }

            var response = await _itemService.CreateItem(dto, ct);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = response.Data!.Id },
                response);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponseDTO<ItemDTO>>> Update(
            Guid id,
            [FromBody] UpdateItemDTO dto,
            CancellationToken ct = default)
        {
            var validation = await _updateItemValidator.ValidateAsync(dto, ct);

            if (!validation.IsValid)
            {
                return BadRequest(ApiResponseDTO<ItemDTO>.ErrorResult(
                    "Validation failed.",
                    validation.Errors
                        .Select(error => error.ErrorMessage)
                        .ToList()));
            }

            var response = await _itemService.UpdateItem(id, dto, ct);

            return response.Success
                ? Ok(response)
                : NotFound(response);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponseDTO<bool>>> Delete(
            Guid id,
            CancellationToken ct = default)
        {
            var response = await _itemService.DeleteItem(id, ct);

            return response.Success
                ? Ok(response)
                : NotFound(response);
        }

        //lot summary
        [HttpGet("item/{itemId:guid}/summary")]
        public async Task<ActionResult<ApiResponseDTO<IEnumerable<LotSummaryDTO>>>> GetSummaryByItemId(
            Guid itemId,
            CancellationToken ct = default)
        {
            var response = await _lotService.GetSummaryByItemId(itemId, ct);
            return Ok(response);
            }
    }
}
