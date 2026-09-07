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

        public DbInitializer(
            AppDbContext dbContext,
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            ILogger<DbInitializer> logger)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task Initialized()
        {
            _logger.LogInformation("Applying PostgreSQL database migrations...");

            await _dbContext.Database.MigrateAsync();

            await UserSeeder.SeedRolesAndSuperAdminAsync(
                _userManager,
                _roleManager
            );

            _logger.LogInformation("PostgreSQL database initialization completed.");
        }
    }
}