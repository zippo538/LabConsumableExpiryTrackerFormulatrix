using AutoMapper;
using LabConsumableExpireTracker.Models;
using LabConsumableExpiryTracker.Data;
using LabConsumableExpiryTracker.DTOs;
using LabConsumableExpiryTracker.Repositories;
using LabConsumableExpiryTracker.Services.Interfaces;
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

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var response = await _lotService.GetAll(ct);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var response = await _lotService.GetById(id, ct);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpGet("item/{itemId:guid}")]
    public async Task<IActionResult> GetByItemId(Guid itemId, CancellationToken ct)
    {
        var response = await _lotService.GetByItemId(itemId, ct);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLotDTO request, CancellationToken ct)
    {
        var response = await _lotService.Create(request, ct);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return CreatedAtAction(nameof(GetById), new { id = response.Data?.Id }, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLotDTO request, CancellationToken ct)
    {
        var response = await _lotService.Update(id, request, ct);
        return response.Success ? NoContent() : NotFound(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var response = await _lotService.Delete(id, ct);
        return response.Success ? NoContent() : NotFound(response);
    }
}

