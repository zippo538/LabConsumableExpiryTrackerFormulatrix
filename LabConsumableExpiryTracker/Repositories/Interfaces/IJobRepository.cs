using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LabConsumableExpiryTracker.Models;

namespace LabConsumableExpiryTracker.Repositories.Interfaces
{
    public interface IJobRepository : IRepository<Job, Guid>
    {
        Task<Job?> GetByJobNumberAsync(string jobNumber, CancellationToken ct = default);
    }
}