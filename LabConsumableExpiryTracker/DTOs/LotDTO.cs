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
        public required string LotNumber { get; init; }
        public DateTimeOffset ReceivedAt { get; init; }
        public decimal InitialQuantity { get; init; }
        public decimal RemainingQuantity { get; init; }
        public DateOnly ExpiryDate { get; init; }
        public required string StorageLocation { get; init; }
        public required string AdministrativeStatus { get; init; }
        public byte[] RowVersion { get; init; } = [];
    }
}