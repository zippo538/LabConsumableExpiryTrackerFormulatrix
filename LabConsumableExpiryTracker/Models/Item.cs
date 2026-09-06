using LabConsumableExpiryTracker.Models.Enums;

namespace LabConsumableExpiryTracker.Models;

public class Item
{
    private readonly List<Lot> _lots = [];
    public Guid Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public UnitOfMeasure BaseUnit { get; private set; }
    public decimal MinimumStock { get; private set; }
    public int ExpiringSoonDays { get; private set; }
    public IReadOnlyCollection<Lot> Lots => _lots.AsReadOnly();


    public Item(
        Guid id,
        string code,
        string name,
        UnitOfMeasure baseUnit,
        decimal minimumStock,
        int expiringSoonDays)
    {
        Id = id;
        Code = code.Trim();
        Name = name.Trim();
        BaseUnit = baseUnit;
        MinimumStock = minimumStock;
        ExpiringSoonDays = expiringSoonDays;
    }

    public void UpdateDetails(
        string name,
        UnitOfMeasure baseUnit,
        decimal minimumStock,
        int expiringSoonDays)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Item name is required.",
                nameof(name));
        }

        if (minimumStock < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(minimumStock));
        }

        if (expiringSoonDays < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expiringSoonDays));
        }

        Name = name.Trim();
        BaseUnit = baseUnit;
        MinimumStock = minimumStock;
        ExpiringSoonDays = expiringSoonDays;
    }
}
