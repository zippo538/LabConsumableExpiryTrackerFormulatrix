using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LabConsumableExpiryTracker.Models.Enums;

namespace LabConsumableExpiryTracker.DTOs
{
    public class LotDto
    {
        public Guid Id { get; init; }
        public Guid ItemId { get; init; }
        public  string LotNumber { get; init; } = null!;
        public DateTimeOffset ReceivedAt { get; init; }
        public decimal InitialQuantity { get; init; }
        public decimal RemainingQuantity { get; set; }
        public DateOnly ExpiryDate { get; init; }
        public  string StorageLocation { get; init; } = null!;
        public  LotStatus Status { get; init; } 
        public byte[] RowVersion { get; init; } = [];
 
    }
}