using LabConsumableExpireTracker.Domain.Common;

namespace LabConsumableExpireTracker.Domain.Entities;

public sealed class Lot
{
    private readonly List<SubLot> _subLots = [];

    private Lot()
    {
    }

    public Lot(
        Guid id,
        Guid itemId,
        string lotNumber,
        string? supplierLotNumber,
        DateTimeOffset receivedAt,
        string? supplierName)
    {
        if (id == Guid.Empty) throw new DomainException("Lot ID is required.");
        if (itemId == Guid.Empty) throw new DomainException("Item ID is required.");
        if (string.IsNullOrWhiteSpace(lotNumber)) throw new DomainException("Lot number is required.");

        Id = id;
        ItemId = itemId;
        LotNumber = lotNumber.Trim();
        SupplierLotNumber = supplierLotNumber?.Trim();
        ReceivedAt = receivedAt;
        SupplierName = supplierName?.Trim();
    }

    public Guid Id { get; private set; }
    public Guid ItemId { get; private set; }
    public string LotNumber { get; private set; } = string.Empty;
    public string? SupplierLotNumber { get; private set; }
    public DateTimeOffset ReceivedAt { get; private set; }
    public string? SupplierName { get; private set; }
    public IReadOnlyCollection<SubLot> SubLots => _subLots.AsReadOnly();
    public decimal RemainingQuantity => _subLots.Sum(x => x.RemainingQuantity);

    public SubLot CreateSubLot(
        Guid subLotId,
        string subLotNumber,
        DateOnly expiryDate,
        decimal quantity,
        string storageLocation,
        DateTimeOffset inspectedAt,
        Guid? inspectedBy = null)
    {
        if (_subLots.Any(x => x.SubLotNumber.Equals(subLotNumber, StringComparison.OrdinalIgnoreCase)))
            throw new DomainException($"Sub-lot number '{subLotNumber}' already exists in this lot.");

        var subLot = new SubLot(
            subLotId,
            Id,
            subLotNumber,
            splitFromSubLotId: null,
            quantity,
            expiryDate,
            storageLocation,
            inspectedAt,
            inspectedBy);

        _subLots.Add(subLot);
        return subLot;
    }

    public SubLot SplitSubLot(
        SubLot source,
        Guid newSubLotId,
        string newSubLotNumber,
        decimal quantity,
        DateOnly expiryDate,
        string storageLocation,
        DateTimeOffset inspectedAt,
        Guid? inspectedBy = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (source.LotId != Id || !_subLots.Contains(source))
            throw new DomainException("The source sub-lot does not belong to this lot.");
        if (_subLots.Any(x => x.SubLotNumber.Equals(newSubLotNumber, StringComparison.OrdinalIgnoreCase)))
            throw new DomainException($"Sub-lot number '{newSubLotNumber}' already exists in this lot.");

        var separated = source.Split(
            newSubLotId,
            newSubLotNumber,
            quantity,
            expiryDate,
            storageLocation,
            inspectedAt,
            inspectedBy);

        _subLots.Add(separated);
        return separated;
    }
}
