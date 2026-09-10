using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LabConsumableExpiryTracker.Data.Seeders;
using LabConsumableExpiryTracker.Services.Interfaces;

namespace LabConsumableExpiryTracker.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }
        public async Task<int> SaveChangesAsync(
        CancellationToken ct = default)
        {
            return await _context.SaveChangesAsync(ct);
        }
    }
}