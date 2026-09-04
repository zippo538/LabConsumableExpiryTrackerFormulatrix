using LabConsumableExpireTracker.Models;
using LabConsumableExpiryTracker.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LabConsumableExpiryTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LotController : ControllerBase
{
    private readonly ILotRepository _lotRepository;

    public LotController(ILotRepository lotRepository)
    {
        _lotRepository = lotRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Lot>>> GetAll(CancellationToken ct)
    {
        var lots = await _lotRepository.GetAllAsync(ct);
        return Ok(lots);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Lot>> GetById(Guid id, CancellationToken ct)
    {
        var lot = await _lotRepository.GetByIdAsync(id, ct);
        if (lot is null) return NotFound();
        return Ok(lot);
    }

    [HttpGet("item/{itemId:guid}")]
    public async Task<ActionResult<IEnumerable<Lot>>> GetByItemId(Guid itemId, CancellationToken ct)
    {
        var lots = await _lotRepository.GetByItemIdAsync(itemId, ct);
        return Ok(lots);
    }

    [HttpPost]
    public async Task<ActionResult<Lot>> Create([FromBody] CreateLotRequest request, CancellationToken ct)
    {
        var lot = new Lot(
            Guid.NewGuid(),
            request.ItemId,
            request.LotNumber,
            request.SupplierLotNumber,
            request.ReceivedAt,
            request.SupplierName
        );

        var created = await _lotRepository.AddAsync(lot, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLotRequest request, CancellationToken ct)
    {
        var existing = await _lotRepository.GetByIdAsync(id, ct);
        if (existing is null) return NotFound();

        var lot = new Lot(
            id,
            request.ItemId,
            request.LotNumber,
            request.SupplierLotNumber,
            request.ReceivedAt,
            request.SupplierName
        );

        await _lotRepository.UpdateAsync(lot, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await _lotRepository.DeleteAsync(id, ct);
        if (!deleted) return NotFound();
        return NoContent();
    }
}

public record CreateLotRequest(
    Guid ItemId,
    string LotNumber,
    string? SupplierLotNumber,
    DateTimeOffset ReceivedAt,
    string? SupplierName
);

public record UpdateLotRequest(
    Guid ItemId,
    string LotNumber,
    string? SupplierLotNumber,
    DateTimeOffset ReceivedAt,
    string? SupplierName
);
