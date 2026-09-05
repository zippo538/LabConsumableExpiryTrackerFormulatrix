using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LabConsumableExpiryTracker.DTOs
{
    public class CreateLotDTO
    {
        public Guid ItemId { get; set; }
        public string LotNumber { get; set; } = string.Empty;
        public string? SupplierLotNumber { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public decimal InitialQuantity { get; set; }
        public DateTime ReceivedAt { get; set; }
        public decimal RemainingQuantity { get; set; }
        public string StorageLocation { get; set; } = string.Empty;
    }
}