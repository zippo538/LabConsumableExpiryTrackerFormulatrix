using LabConsumableExpiryTracker.Models.Enums;

namespace LabConsumableExpiryTracker.Models;

public class Lot
{

    public Guid Id { get; private set; }
    public Guid ItemId { get; private set; }
    public string LotNumber { get; private set; } = string.Empty;
    public string? SupplierLotNumber { get; private set; }
    public DateTimeOffset ReceivedAt { get; private set; }
    public string? SupplierName { get; private set; }
    public decimal InitialQuantity { get; private set; }
    public decimal RemainingQuantity { get; private set; }
    public DateOnly ExpiryDate { get; private set; }
    public string StorageLocation { get; private set; } = string.Empty;
    public LotStatus Status { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    public Lot(
    Guid id,
    Guid itemId,
    string lotNumber,
    string? supplierLotNumber,
    DateTimeOffset receivedAt,
    string? supplierName,
    decimal initialQuantity,
    decimal remainingQuantity,
    DateOnly expiryDate,
    string storageLocation,
    LotStatus status)
    {
        Id = id;
        ItemId = itemId;
        LotNumber = lotNumber.Trim();
        SupplierLotNumber = supplierLotNumber?.Trim();
        ReceivedAt = receivedAt;
        SupplierName = supplierName?.Trim();
        InitialQuantity = initialQuantity;
        RemainingQuantity = remainingQuantity;
        ExpiryDate = expiryDate;
        StorageLocation = storageLocation.Trim();
        Status = status;
    }
    public void UpdateDetails(
        decimal remainingQuantity,
        string storageLocation)
    {
        if (remainingQuantity < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(remainingQuantity),
                "Remaining quantity cannot be negative.");
        }

        if (remainingQuantity > InitialQuantity)
        {
            throw new ArgumentException(
                "Remaining quantity cannot exceed initial quantity.",
                nameof(remainingQuantity));
        }

        if (string.IsNullOrWhiteSpace(storageLocation))
        {
            throw new ArgumentException(
                "Storage location is required.",
                nameof(storageLocation));
        }

        RemainingQuantity = remainingQuantity;
        StorageLocation = storageLocation.Trim();
    }
}

