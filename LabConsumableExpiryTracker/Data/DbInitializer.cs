using Microsoft.EntityFrameworkCore;

namespace LabConsumableExpiryTracker.Data
{
    public class DbInitializer : IDbinitializer
    {
        private readonly AppDbContext _dbContext;
    private readonly ILogger<DbInitializer> _logger;

    public DbInitializer(
        AppDbContext dbContext,
        ILogger<DbInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Initialized()
    {
        _logger.LogInformation("Applying PostgreSQL database migrations...");

        await _dbContext.Database.MigrateAsync();

        _logger.LogInformation("PostgreSQL database initialization completed.");
    }
    }
}