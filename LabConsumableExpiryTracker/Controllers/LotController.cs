using LabConsumableExpiryTracker.Commons.Result;
using LabConsumableExpiryTracker.DTOs;
using LabConsumableExpiryTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabConsumableExpiryTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LotController : ControllerBase
{
    private readonly ILotService _lotService;

    public LotController(ILotService lotService)
    {
        _lotService = lotService;
    }

    [Authorize(Roles = "WarehouseAdmin")]
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var response = await _lotService.GetAllLot(ct);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [Authorize(Roles = "WarehouseAdmin")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var response = await _lotService.GetByIdLot(id, ct);
        return response.Success ? Ok(response) : NotFound(response);
    }



    [Authorize(Roles = "WarehouseAdmin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLotDTO request, CancellationToken ct)
    {
        var response = await _lotService.CreateLot(request, ct);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return CreatedAtAction(nameof(GetById), new { id = response.Data?.Id }, response);
    }

    [Authorize(Roles = "WarehouseAdmin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLotDTO request, CancellationToken ct)
    {
        var response = await _lotService.UpdateLot(id, request, ct);
        return response.Success ? NoContent() : NotFound(response);
    }

    [Authorize(Roles = "WarehouseAdmin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var response = await _lotService.DeleteLot(id, ct);
        return response.Success ? NoContent() : NotFound(response);
    }
    
    [Authorize(Roles = "WarehouseAdmin")]
    [HttpGet("summary")]
    public async Task<ActionResult<ServiceResult<IEnumerable<LotSummaryDTO>>>> GetAllSummary(
    CancellationToken ct = default)
    {
    var response = await _lotService.GetAllSummary(ct);
    return Ok(response);
    }
}

