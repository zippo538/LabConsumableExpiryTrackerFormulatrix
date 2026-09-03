using LabConsumableExpireTracker.Domain.Common;

namespace LabConsumableExpireTracker.Domain.Entities;

public sealed class Consumption
{
    private Consumption()
    {
    }

    internal Consumption(
        Guid id,
        Guid jobId,
        Guid subLotId,
        decimal quantity,
        DateTimeOffset consumedAt,
        Guid consumedBy)
    {
        if (id == Guid.Empty || jobId == Guid.Empty || subLotId == Guid.Empty || consumedBy == Guid.Empty)
            throw new DomainException("Consumption identifiers are required.");
        if (quantity <= 0) throw new DomainException("Consumption quantity must be greater than zero.");

        Id = id;
        JobId = jobId;
        SubLotId = subLotId;
        Quantity = quantity;
        ConsumedAt = consumedAt;
        ConsumedBy = consumedBy;
    }

    public Guid Id { get; private set; }
    public Guid JobId { get; private set; }
    public Guid SubLotId { get; private set; }
    public decimal Quantity { get; private set; }
    public DateTimeOffset ConsumedAt { get; private set; }
    public Guid ConsumedBy { get; private set; }
}
