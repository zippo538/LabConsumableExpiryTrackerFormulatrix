using Bogus;
using LabConsumableExpiryTracker.Models;
using LabConsumableExpiryTracker.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace LabConsumableExpiryTracker.Data.Seeders
{
    public class ItemLotSeeder
    {
        private readonly AppDbContext _dbContext;

        public ItemLotSeeder(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task SeedAsync(CancellationToken ct = default)
        {
            var faker = new Faker("en");

            var units = new[]
            {
        UnitOfMeasure.Milliliter,
        UnitOfMeasure.Gram,
        UnitOfMeasure.Unit,
        UnitOfMeasure.Vial
    };

            var items = new List<Item>();

            for (var index = 1; index <= 10; index++)
            {
                var code = $"ITEM-{index:000}";

                var item = await _dbContext.Items
                    .FirstOrDefaultAsync(x => x.Code == code, ct);

                if (item is null)
                {
                    item = new Item(
                        Guid.NewGuid(),
                        code,
                        faker.Commerce.ProductName(),
                        units[(index - 1) % units.Length],
                        faker.Random.Decimal(5, 50),
                        faker.Random.Int(30, 90));

                    await _dbContext.Items.AddAsync(item, ct);
                }

                items.Add(item);
            }

            await _dbContext.SaveChangesAsync(ct);

            foreach (var item in items)
            {
                var hasLots = await _dbContext.Lots
                    .AnyAsync(lot => lot.ItemId == item.Id, ct);

                if (hasLots)
                {
                    continue;
                }

                var lotCount = faker.Random.Int(2, 4);

                for (var index = 1; index <= lotCount; index++)
                {
                    var initialQuantity = faker.Random.Decimal(25, 250);
                    var remainingQuantity = faker.Random.Decimal(
                        0,
                        initialQuantity);

                    var lot = new Lot(
                        Guid.NewGuid(),
                        item.Id,
                        $"LOT-{item.Code}-{index:000}",
                        $"SUP-{faker.Random.Int(1000, 9999)}",
                        DateTimeOffset.UtcNow.AddDays(
                            -faker.Random.Int(1, 180)),
                        faker.Company.CompanyName(),
                        initialQuantity,
                        remainingQuantity,
                        DateOnly.FromDateTime(
                            DateTime.UtcNow.Date.AddDays(
                                faker.Random.Int(30, 730))),
                        $"Rack-{faker.Random.Int(1, 8)}",
                        LotStatus.Active);

                    await _dbContext.Lots.AddAsync(lot, ct);
                }
            }

            await _dbContext.SaveChangesAsync(ct);
        }
    }
}