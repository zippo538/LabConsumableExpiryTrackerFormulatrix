using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LabConsumableExpiryTracker.DTOs
{
    public class LotDTO
    {
        public Guid Id { get; init; }
        public Guid ItemId { get; init; }
        public  string LotNumber { get; init; } = null!;
        public DateTimeOffset ReceivedAt { get; init; }
        public decimal InitialQuantity { get; init; }
        public decimal RemainingQuantity { get; init; }
        public DateOnly ExpiryDate { get; init; }
        public  string StorageLocation { get; init; } = null!;
        public  string AdministrativeStatus { get; init; } = null!;
        public byte[] RowVersion { get; init; } = [];
    }
}