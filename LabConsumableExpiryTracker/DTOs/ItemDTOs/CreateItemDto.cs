using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LabConsumableExpiryTracker.Models.Enums;

namespace LabConsumableExpiryTracker.DTOs
{
    public class CreateItemDto
    {
        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public UnitOfMeasure BaseUnit { get; set; }

        public decimal MinimumStock { get; set; }

        public int ExpiringSoonDays { get; set; }

    }
}