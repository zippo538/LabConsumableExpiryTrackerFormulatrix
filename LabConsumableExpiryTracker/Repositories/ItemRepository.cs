using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LabConsumableExpiryTracker.Models;
using LabConsumableExpiryTracker.Data;
using LabConsumableExpiryTracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LabConsumableExpiryTracker.Repositories
{
    public class ItemRepository : Repository<Item, Guid>, IItemRepository
    {
        private readonly AppDbContext _context;
        public ItemRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Item?> GetByCodeAsync(
       string code,
       CancellationToken ct = default)
        {
            return await _context.Items
                .FirstOrDefaultAsync(
                    item => item.Code == code,
                    ct);
        }
    }
}