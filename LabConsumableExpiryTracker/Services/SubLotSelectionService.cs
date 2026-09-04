using LabConsumableExpireTracker.Domain.Common;
using LabConsumableExpireTracker.Domain.Entities;

namespace LabConsumableExpireTracker.Domain.Services;

public sealed class SubLotSelectionService
{
    private readonly TimeProvider _timeProvider;

    public SubLotSelectionService(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    }

    public IReadOnlyList<SubLotAllocation> Allocate(
        IEnumerable<SubLot> subLots,
        decimal requestedQuantity)
    {
        ArgumentNullException.ThrowIfNull(subLots);
        if (requestedQuantity <= 0)
            throw new DomainException("Requested quantity must be greater than zero.");

        var today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
        var remainingRequest = requestedQuantity;
        var allocations = new List<SubLotAllocation>();

        var candidates = subLots
            .Where(x => x.IsEligible(today))
            .OrderBy(x => x.ExpiryDate)
            .ThenBy(x => x.InspectedAt)
            .ThenBy(x => x.SubLotNumber, StringComparer.OrdinalIgnoreCase);

        foreach (var subLot in candidates)
        {
            var allocatedQuantity = Math.Min(subLot.RemainingQuantity, remainingRequest);
            allocations.Add(new SubLotAllocation(subLot.Id, allocatedQuantity));
            remainingRequest -= allocatedQuantity;

            if (remainingRequest == 0) break;
        }

        if (remainingRequest > 0)
            throw new DomainException(
                $"Eligible stock is insufficient. Shortage: {remainingRequest}.");

        return allocations;
    }
}
