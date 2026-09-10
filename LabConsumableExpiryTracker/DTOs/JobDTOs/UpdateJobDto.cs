using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LabConsumableExpiryTracker.Models.Enums;

namespace LabConsumableExpiryTracker.DTOs.JobDTOs
{
    public class UpdateJobDto
    {
        public JobStatus Status { get; init; }
    }
}