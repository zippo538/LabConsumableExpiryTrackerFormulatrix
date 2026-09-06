using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LabConsumableExpiryTracker.DTOs
{
    public class LotSummaryDTO
    {
        public Guid LotId { get; init; }
        public required string LotNumber { get; init; }
        public decimal RemainingQuantity { get; init; }
        public DateOnly ExpiryDate { get; init; }
        public required string Status { get; init; }
        
    }
}