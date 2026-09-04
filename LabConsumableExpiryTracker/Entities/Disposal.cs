using LabConsumableExpireTracker.Domain.Common;

namespace LabConsumableExpireTracker.Domain.Entities;

public sealed class Disposal
{
    private Disposal()
    {
    }

    internal Disposal(
        Guid id,
        Guid subLotId,
        decimal quantity,
        string reason,
        DateTimeOffset disposedAt,
        Guid disposedBy)
    {
        if (id == Guid.Empty || subLotId == Guid.Empty || disposedBy == Guid.Empty)
            throw new DomainException("Disposal identifiers are required.");
        if (quantity <= 0) throw new DomainException("Disposal quantity must be greater than zero.");
        if (string.IsNullOrWhiteSpace(reason)) throw new DomainException("Disposal reason is required.");

        Id = id;
        SubLotId = subLotId;
        Quantity = quantity;
        Reason = reason.Trim();
        DisposedAt = disposedAt;
        DisposedBy = disposedBy;
    }

    public Guid Id { get; private set; }
    public Guid SubLotId { get; private set; }
    public decimal Quantity { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public DateTimeOffset DisposedAt { get; private set; }
    public Guid DisposedBy { get; private set; }
}
