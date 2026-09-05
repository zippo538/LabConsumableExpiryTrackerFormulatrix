using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LabConsumableExpiryTracker.DTOs
{
    public class UpdateLotDTO
    {
        public decimal RemainingQuantity { get; set; }
        public string StorageLocation { get; set; } = string.Empty;
    }
}