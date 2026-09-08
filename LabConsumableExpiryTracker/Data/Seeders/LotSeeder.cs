using Bogus;
using LabConsumableExpiryTracker.Models;
using LabConsumableExpiryTracker.Models.Enums;

namespace LabConsumableExpiryTracker.Data.Seeders
{
    public class LotSeeder
    {
        private static readonly int[] ExpiryDayOffsets = [-30, 7, 30, 180];

        public static IReadOnlyCollection<Lot> Generate(
            IEnumerable<Guid> itemIds,
            TimeProvider timeProvider,
            int lotsPerItem = 4,
            int seed = 20260908)
        {
            ArgumentNullException.ThrowIfNull(itemIds);
            ArgumentNullException.ThrowIfNull(timeProvider);

            if (lotsPerItem <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(lotsPerItem),
                    "Lots per item must be greater than zero.");
            }

            var validItemIds = itemIds
                .Where(itemId => itemId != Guid.Empty)
                .Distinct()
                .ToArray();

            if (validItemIds.Length == 0)
            {
                return [];
            }

            var faker = new Faker("id_ID");
            faker.Random = new Randomizer(seed);

            var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
            var lots = new List<Lot>(validItemIds.Length * lotsPerItem);

            for (var itemIndex = 0; itemIndex < validItemIds.Length; itemIndex++)
            {
                for (var lotIndex = 0; lotIndex < lotsPerItem; lotIndex++)
                {
                    var initialQuantity = decimal.Round(
                        faker.Random.Decimal(25m, 500m),
                        2,
                        MidpointRounding.AwayFromZero);

                    var remainingQuantity = decimal.Round(
                        faker.Random.Decimal(0m, initialQuantity),
                        2,
                        MidpointRounding.AwayFromZero);

                    var expiryOffset = ExpiryDayOffsets[
                        lotIndex % ExpiryDayOffsets.Length];

                    var expiryDate = today.AddDays(
                        expiryOffset + faker.Random.Int(0, 5));

                    var receivedAt = new DateTimeOffset(
                            today.ToDateTime(new TimeOnly(8, 0)),
                            TimeSpan.Zero)
                        .AddDays(-faker.Random.Int(7, 365));

                    var lotNumber = $"LOT-{itemIndex + 1:D3}-{lotIndex + 1:D3}";
                    var supplierLotNumber = faker.Random.Bool(0.8f)
                        ? $"SUP-{faker.Random.AlphaNumeric(8).ToUpperInvariant()}"
                        : null;

                    lots.Add(new Lot(
                        faker.Random.Guid(),
                        validItemIds[itemIndex],
                        lotNumber,
                        supplierLotNumber,
                        receivedAt,
                        faker.Company.CompanyName(),
                        initialQuantity,
                        remainingQuantity,
                        expiryDate,
                        $"Rack-{faker.Random.Int(1, 8):D2}",
                        LotStatus.Active));
                }
            }

            return lots;
        }
    }
}