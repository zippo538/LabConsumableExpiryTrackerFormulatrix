using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using LabConsumableExpiryTracker.Models;
using LabConsumableExpiryTracker.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace LabConsumableExpiryTracker.Data.Seeders
{
    public class ItemLotSeeder
    {
        private readonly AppDBContext _dbContext;

        public ItemLotSeeder(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task SeedAsync(CancellationToken ct = default)
        {
            if (await _dbContext.Items.AnyAsync(ct))
            {
                return;
            }

            var faker = new Faker("en");
            var units = new[]
            {
            UnitOfMeasure.Milliliter,
            UnitOfMeasure.Gram,
            UnitOfMeasure.Unit,
            UnitOfMeasure.Vial
        };

            var items = Enumerable.Range(1, 10)
                .Select(index => new Item(
                    Guid.NewGuid(),
                    $"ITEM-{index:000}",
                    faker.Commerce.ProductName(),
                    units[(index - 1) % units.Length],
                    faker.Random.Decimal(5, 50),
                    faker.Random.Int(30, 90)))
                .ToList();

            var lots = new List<Lot>();

            foreach (var item in items)
            {
                var lotCount = faker.Random.Int(2, 4);

                for (var index = 1; index <= lotCount; index++)
                {
                    var initialQuantity = faker.Random.Decimal(25, 250);
                    var remainingQuantity = faker.Random.Decimal(0, initialQuantity);

                    lots.Add(new Lot(
                        Guid.NewGuid(),
                        item.Id,
                        $"LOT-{item.Code}-{index:000}",
                        $"SUP-{faker.Random.Int(1000, 9999)}",
                        DateTimeOffset.UtcNow.AddDays(-faker.Random.Int(1, 180)),
                        faker.Company.CompanyName(),
                        initialQuantity,
                        remainingQuantity,
                        DateOnly.FromDateTime(
                            DateTime.UtcNow.Date.AddDays(faker.Random.Int(30, 730))),
                        $"Rack-{faker.Random.Int(1, 8)}",
                        LotStatus.Active));
                }
            }

            await using var transaction = await _dbContext.Database
                .BeginTransactionAsync(ct);

            await _dbContext.Items.AddRangeAsync(items, ct);
            await _dbContext.Lots.AddRangeAsync(lots, ct);
            await _dbContext.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }
    }
}