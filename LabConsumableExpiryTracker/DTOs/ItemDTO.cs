using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LabConsumableExpireTracker.Models.Enums;

namespace LabConsumableExpiryTracker.DTOs
{
    public class ItemDTO
    {
     public Guid Id { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public UnitOfMeasure BaseUnit { get; init; }
    public decimal MinimumStock { get; init; }
    public int ExpiringSoonDays { get; init; }
    public IReadOnlyCollection<LotSummaryDTO> Lots { get; init; } = [];   
    }
}