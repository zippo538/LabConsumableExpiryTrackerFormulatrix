using LabConsumableExpireTracker.Models.Enums;

namespace LabConsumableExpireTracker.Models;

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
    public SubLotStatus Status { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    
    public Lot(
        Guid id,
        Guid itemId,
        string lotNumber,
        string? supplierLotNumber,
        DateTimeOffset receivedAt,
        string? supplierName
        )
    {
        Id = id;
        ItemId = itemId;
        LotNumber = lotNumber.Trim();
        SupplierLotNumber = supplierLotNumber?.Trim();
        ReceivedAt = receivedAt;
        SupplierName = supplierName?.Trim();   
    }


    
    }

