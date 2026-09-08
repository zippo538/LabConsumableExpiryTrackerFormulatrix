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

            // Data spesifik Formulatrix (4 Kategori, masing-masing 5 item)
            var seedData = new List<(string Name, string Code, UnitOfMeasure Unit)>
            {
                // Kategori 1: Reagen & Kit PCR / dPCR
                ("CONSTELLATION® dPCR Master Mix 2X", "DPCR-MM-001", UnitOfMeasure.Milliliter),
                ("Nuclease-Free Water (PCR Grade)", "DPCR-NFW-002", UnitOfMeasure.Milliliter),
                ("dPCR Partitioning Oil", "DPCR-OIL-003", UnitOfMeasure.Milliliter),
                ("Positive Control DNA Standard", "DPCR-STD-004", UnitOfMeasure.Vial),
                ("FAM-labeled TaqMan Probe", "DPCR-PRB-005", UnitOfMeasure.Vial),

                // Kategori 2: Konsumabel & Microfluidics
                ("Mantis® High Volume Silicone Chip", "MF-MANT-001", UnitOfMeasure.Unit),
                ("Mantis® Low Volume PFE Chip", "MF-MANT-002", UnitOfMeasure.Unit),
                ("µPulse® TFF Concentration Chip", "MF-TFF-003", UnitOfMeasure.Unit),
                ("CONSTELLATION® 96-Well Microplate", "MF-PLT-004", UnitOfMeasure.Unit),
                ("Tempest® 96-Channel Dispense Head", "MF-TMP-005", UnitOfMeasure.Unit),

                // Kategori 3: Reagen Kultur Sel & Dekontaminasi
                ("DMEM High Glucose Cell Media", "CELL-MED-001", UnitOfMeasure.Milliliter),
                ("Fetal Bovine Serum (FBS)", "CELL-FBS-002", UnitOfMeasure.Milliliter),
                ("Trypsin-EDTA 0.25%", "CELL-TRY-003", UnitOfMeasure.Milliliter),
                ("Puroxia® Hydrogen Peroxide (H2O2) 35%", "CELL-PRX-004", UnitOfMeasure.Milliliter),
                ("Penicillin-Streptomycin Solution", "CELL-PEN-005", UnitOfMeasure.Milliliter),

                // Kategori 4: Reagen & Screen Kristalisasi Protein
                ("LCP Monoolein Substrate (99%)", "XTAL-LCP-001", UnitOfMeasure.Gram),
                ("PEG 3350 Crystallization Screen Kit", "XTAL-SCR-002", UnitOfMeasure.Vial),
                ("NT8® 96-Well Hanging Drop Plate", "XTAL-PLT-003", UnitOfMeasure.Unit),
                ("HEPES Buffer Solution pH 7.5", "XTAL-BUF-004", UnitOfMeasure.Milliliter),
                ("Ammonium Sulfate Precipitant", "XTAL-PPT-005", UnitOfMeasure.Milliliter)
            };

            var items = new List<Item>();

            // 1. Seed Items
            foreach (var data in seedData)
            {
                var item = await _dbContext.Items
                    .FirstOrDefaultAsync(x => x.Code == data.Code, ct);

                if (item is null)
                {
                    decimal minStock = data.Unit == UnitOfMeasure.Unit
                        ? faker.Random.Decimal(10, 100)
                        : faker.Random.Decimal(100, 1000);

                    item = new Item(
                        Guid.NewGuid(),
                        data.Code,
                        data.Name,
                        data.Unit,
                        minStock,
                        faker.Random.Int(14, 90));

                    await _dbContext.Items.AddAsync(item, ct);
                }

                items.Add(item);
            }

            await _dbContext.SaveChangesAsync(ct);

            // 2. Seed Lots untuk setiap Item yang ada
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