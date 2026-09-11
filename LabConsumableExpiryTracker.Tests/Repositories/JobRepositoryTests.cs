using FluentAssertions;
using LabConsumableExpiryTracker.Data.Seeders;
using LabConsumableExpiryTracker.Models;
using LabConsumableExpiryTracker.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LabConsumableExpiryTracker.Tests.Repositories
{
    public class JobRepositoryTests
    {
        private AppDbContext _context;
        private JobRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options =
                new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(
                    "Host=localhost;" +
                    "Port=5432;" +
                    "Database=LabConsumableExpiryTracker_Test;" +
                    "Username=postgres;" +
                    "Password=qwerty123")
                .Options;


            _context =
                new AppDbContext(options);


            _context.Database.EnsureCreated();


            _repository =
                new JobRepository(_context);
        }



        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }



        [Test]
        public async Task AddAsync_Should_Add_New_Job()
        {
            var job =
                new Job(
                    Guid.NewGuid(),
                    "JOB-001");


            var result =
                await _repository.AddAsync(job);


            await _context.SaveChangesAsync();



            result.Should()
                .NotBeNull();


            var saved =
                await _context.Jobs
                .FirstOrDefaultAsync(
                    x => x.JobNumber == "JOB-001");


            saved.Should()
                .NotBeNull();
        }
    }
}
