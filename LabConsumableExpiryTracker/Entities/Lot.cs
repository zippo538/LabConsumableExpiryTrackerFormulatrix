using LabConsumableExpireTracker.Domain.Common;
using LabConsumableExpireTracker.Domain.Enums;

namespace LabConsumableExpireTracker.Domain.Entities;

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
        string? supplierName,

        
        )
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


    
    }

