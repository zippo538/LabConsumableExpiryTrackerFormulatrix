using LabConsumableExpiryTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LabConsumableExpiryTracker.Data.Seeders
{
    public class DbInitializer : IDbinitializer
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ILogger<DbInitializer> _logger;
        private readonly TimeProvider _timeProvider;

        public DbInitializer(
            AppDbContext dbContext,
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            ILogger<DbInitializer> logger,
            TimeProvider timeProvider)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _timeProvider = timeProvider;
        }

        public async Task Initialized()
        {
            _logger.LogInformation("Applying PostgreSQL database migrations...");

            await _dbContext.Database.MigrateAsync();
            if (!_dbContext.Users.Any())
            {
                await UserSeeder.SeedRolesAndSuperAdminAsync(
                    _userManager,
                    _roleManager
                );
            }
            var generatedItems = ItemSeeder.Generate();

            var existingItemCodes = (await _dbContext.Items
                    .AsNoTracking()
                    .Select(item => item.Code)
                    .ToListAsync())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var newItems = generatedItems
                .Where(item => !existingItemCodes.Contains(item.Code))
                .ToList();

            if (newItems.Count > 0)
            {
                await _dbContext.Items.AddRangeAsync(newItems);
                await _dbContext.SaveChangesAsync();
            }

            var seedItemCodes = generatedItems
                .Select(item => item.Code)
                .ToArray();

            var itemIds = await _dbContext.Items
                .AsNoTracking()
                .Where(item => seedItemCodes.Contains(item.Code))
                .Select(item => item.Id)
                .ToListAsync();

            var itemIdsWithLots = (await _dbContext.Lots
                    .AsNoTracking()
                    .Select(lot => lot.ItemId)
                    .Distinct()
                    .ToListAsync())
                .ToHashSet();

            var itemIdsWithoutLots = itemIds
                .Where(itemId => !itemIdsWithLots.Contains(itemId))
                .ToArray();

            if (itemIdsWithoutLots.Length > 0)
            {
                var lots = LotSeeder.Generate(
                    itemIdsWithoutLots,
                    _timeProvider,
                    lotsPerItem: 4);

                await _dbContext.Lots.AddRangeAsync(lots);
                await _dbContext.SaveChangesAsync();
            }

            _logger.LogInformation("PostgreSQL database initialization completed.");
        }
    }
}