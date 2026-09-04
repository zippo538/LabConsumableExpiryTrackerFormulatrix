using LabConsumableExpireTracker.Domain.Common;
using LabConsumableExpireTracker.Domain.Enums;

namespace LabConsumableExpireTracker.Domain.Entities;

public sealed class Item
{
    private readonly List<Lot> _lots = [];

    private Item()
    {
    }

    public Item(
        Guid id,
        string code,
        string name,
        UnitOfMeasure baseUnit,
        decimal minimumStock,
        int expiringSoonDays)
    {
        if (id == Guid.Empty) throw new DomainException("Item ID is required.");
        if (string.IsNullOrWhiteSpace(code)) throw new DomainException("Item code is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Item name is required.");
        if (minimumStock < 0) throw new DomainException("Minimum stock cannot be negative.");
        if (expiringSoonDays < 0) throw new DomainException("Expiring-soon days cannot be negative.");

        Id = id;
        Code = code.Trim();
        Name = name.Trim();
        BaseUnit = baseUnit;
        MinimumStock = minimumStock;
        ExpiringSoonDays = expiringSoonDays;
    }

    public Guid Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public UnitOfMeasure BaseUnit { get; private set; }
    public decimal MinimumStock { get; private set; }
    public int ExpiringSoonDays { get; private set; }
    public IReadOnlyCollection<Lot> Lots => _lots.AsReadOnly();

    public bool IsLowStock(decimal totalQuantity)
    {
        if (totalQuantity < 0) throw new DomainException("Total quantity cannot be negative.");
        return totalQuantity < MinimumStock;
    }
    public bool RequiresApproval { get; private set; }
    public decimal? MaximumAutomaticIssueQuantity { get; private set; }
    public bool IsControlledMaterial { get; private set; }

    public void AddLot(Lot lot)
    {
        ArgumentNullException.ThrowIfNull(lot);
        if (lot.ItemId != Id) throw new DomainException("The lot belongs to another item.");
        if (_lots.Any(x => x.LotNumber.Equals(lot.LotNumber, StringComparison.OrdinalIgnoreCase)))
            throw new DomainException($"Lot number '{lot.LotNumber}' already exists for this item.");

        _lots.Add(lot);
    }
}
