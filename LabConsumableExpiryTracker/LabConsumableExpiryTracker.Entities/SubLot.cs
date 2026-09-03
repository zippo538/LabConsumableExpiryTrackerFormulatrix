using LabConsumableExpireTracker.Domain.Common;
using LabConsumableExpireTracker.Domain.Enums;

namespace LabConsumableExpireTracker.Domain.Entities;

public sealed class SubLot
{
    private SubLot()
    {
    }

    internal SubLot(
        Guid id,
        Guid lotId,
        string subLotNumber,
        Guid? splitFromSubLotId,
        decimal quantity,
        DateOnly expiryDate,
        string storageLocation,
        DateTimeOffset inspectedAt,
        Guid? inspectedBy)
    {
        if (id == Guid.Empty) throw new DomainException("Sub-lot ID is required.");
        if (lotId == Guid.Empty) throw new DomainException("Lot ID is required.");
        if (string.IsNullOrWhiteSpace(subLotNumber)) throw new DomainException("Sub-lot number is required.");
        if (quantity <= 0) throw new DomainException("Sub-lot quantity must be greater than zero.");
        if (string.IsNullOrWhiteSpace(storageLocation))
            throw new DomainException("Storage location is required.");

        Id = id;
        LotId = lotId;
        SubLotNumber = subLotNumber.Trim();
        SplitFromSubLotId = splitFromSubLotId;
        InitialQuantity = quantity;
        RemainingQuantity = quantity;
        ExpiryDate = expiryDate;
        StorageLocation = storageLocation.Trim();
        Status = SubLotStatus.Active;
        InspectedAt = inspectedAt;
        InspectedBy = inspectedBy;
    }

    public Guid Id { get; private set; }
    public Guid LotId { get; private set; }
    public string SubLotNumber { get; private set; } = string.Empty;
    public Guid? SplitFromSubLotId { get; private set; }
    public decimal InitialQuantity { get; private set; }
    public decimal RemainingQuantity { get; private set; }
    public DateOnly ExpiryDate { get; private set; }
    public string StorageLocation { get; private set; } = string.Empty;
    public SubLotStatus Status { get; private set; }
    public string? StatusReason { get; private set; }
    public DateTimeOffset InspectedAt { get; private set; }
    public Guid? InspectedBy { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    public ExpiryCondition GetExpiryCondition(DateOnly today, int warningDays)
    {
        if (warningDays < 0) throw new DomainException("Warning days cannot be negative.");
        if (ExpiryDate < today) return ExpiryCondition.Expired;
        return ExpiryDate <= today.AddDays(warningDays)
            ? ExpiryCondition.ExpiringSoon
            : ExpiryCondition.Valid;
    }

    public bool IsEligible(DateOnly today) =>
        Status == SubLotStatus.Active &&
        RemainingQuantity > 0 &&
        ExpiryDate >= today;

    public Consumption Consume(
        decimal quantity,
        Guid jobId,
        Guid consumedBy,
        DateTimeOffset now)
    {
        EnsurePositiveQuantity(quantity);
        if (jobId == Guid.Empty) throw new DomainException("Job ID is required.");
        if (consumedBy == Guid.Empty) throw new DomainException("Consumer ID is required.");
        if (!IsEligible(DateOnly.FromDateTime(now.UtcDateTime)))
            throw new DomainException("The sub-lot is not eligible for consumption.");
        EnsureSufficientQuantity(quantity);

        RemainingQuantity -= quantity;
        if (RemainingQuantity == 0)
        {
            Status = SubLotStatus.Depleted;
            StatusReason = "All stock has been consumed.";
        }

        return new Consumption(Guid.NewGuid(), jobId, Id, quantity, now, consumedBy);
    }

    public Disposal Dispose(
        decimal quantity,
        string reason,
        Guid disposedBy,
        DateTimeOffset now)
    {
        EnsurePositiveQuantity(quantity);
        if (string.IsNullOrWhiteSpace(reason)) throw new DomainException("Disposal reason is required.");
        if (disposedBy == Guid.Empty) throw new DomainException("Disposer ID is required.");
        if (Status is SubLotStatus.Disposed or SubLotStatus.Depleted)
            throw new DomainException("The sub-lot has no stock available for disposal.");
        EnsureSufficientQuantity(quantity);

        RemainingQuantity -= quantity;
        if (RemainingQuantity == 0)
        {
            Status = SubLotStatus.Disposed;
            StatusReason = reason.Trim();
        }

        return new Disposal(Guid.NewGuid(), Id, quantity, reason, now, disposedBy);
    }

    internal SubLot Split(
        Guid newSubLotId,
        string newSubLotNumber,
        decimal quantity,
        DateOnly expiryDate,
        string storageLocation,
        DateTimeOffset inspectedAt,
        Guid? inspectedBy = null)
    {
        EnsurePositiveQuantity(quantity);
        EnsureSufficientQuantity(quantity);
        if (quantity == RemainingQuantity)
            throw new DomainException("A split quantity must be less than the source sub-lot balance.");
        if (Status is SubLotStatus.Disposed or SubLotStatus.Depleted)
            throw new DomainException("A closed sub-lot cannot be split.");

        RemainingQuantity -= quantity;

        return new SubLot(
            newSubLotId,
            LotId,
            newSubLotNumber,
            Id,
            quantity,
            expiryDate,
            storageLocation,
            inspectedAt,
            inspectedBy);
    }

    public void Quarantine(string reason)
    {
        EnsureOpenStock();
        SetRestrictedStatus(SubLotStatus.Quarantined, reason);
    }

    public void Block(string reason)
    {
        EnsureOpenStock();
        SetRestrictedStatus(SubLotStatus.ManuallyBlocked, reason);
    }

    public void Activate()
    {
        EnsureOpenStock();
        Status = SubLotStatus.Active;
        StatusReason = null;
    }

    private static void EnsurePositiveQuantity(decimal quantity)
    {
        if (quantity <= 0) throw new DomainException("Quantity must be greater than zero.");
    }

    private void EnsureSufficientQuantity(decimal quantity)
    {
        if (quantity > RemainingQuantity)
            throw new DomainException("Quantity exceeds the remaining sub-lot stock.");
    }

    private void EnsureOpenStock()
    {
        if (RemainingQuantity <= 0 || Status is SubLotStatus.Disposed or SubLotStatus.Depleted)
            throw new DomainException("The sub-lot is already closed.");
    }

    private void SetRestrictedStatus(SubLotStatus status, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason)) throw new DomainException("Status reason is required.");
        Status = status;
        StatusReason = reason.Trim();
    }
}
