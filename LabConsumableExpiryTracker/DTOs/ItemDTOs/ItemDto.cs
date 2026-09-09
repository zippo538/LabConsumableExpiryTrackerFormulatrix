using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LabConsumableExpiryTracker.Models.Enums;

namespace LabConsumableExpiryTracker.DTOs
{
    public class ItemDto
    {
        public Guid Id { get; init; }

        public string Code { get; init; } = string.Empty;

        public string Name { get; init; } = string.Empty;

        public UnitOfMeasure BaseUnit { get; init; }

        public decimal MinimumStock { get; init; }

        public decimal TotalRemainingQuantity { get; init; }

        public bool IsLowStock { get; init; }

        public IReadOnlyCollection<LotSummaryDto> Lots { get; init; }
        = Array.Empty<LotSummaryDto>();
    }
}