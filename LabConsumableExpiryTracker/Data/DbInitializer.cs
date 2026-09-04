using home.mahindra.RiderProjects.LatihanEFCore.LatihanEFCore.Data;
using Microsoft.EntityFrameworkCore;

namespace LabConsumableExpiryTracker.Data
{
    public class DbInitializer : IDbinitializer
    {
        private readonly AppDBContext _dbContext;
    private readonly ILogger<DbInitializer> _logger;

    public DbInitializer(
        AppDBContext dbContext,
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