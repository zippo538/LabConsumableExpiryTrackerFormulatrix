using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LabConsumableExpiryTracker.Models.Enums;

namespace LabConsumableExpiryTracker.DTOs.JobDTOs
{
    public class JobDto
    {
        public Guid Id { get; init; }
        public string JobNumber { get; init; } = string.Empty;
        public JobStatus Status { get; init; }
        public DateTimeOffset? StartedAt { get; init; }
        public DateTimeOffset? CompletedAt { get; init; }
    }
}