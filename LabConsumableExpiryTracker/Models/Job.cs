using LabConsumableExpireTracker.Models.Enums;

namespace LabConsumableExpireTracker.Models;

public  class Job
{
    private readonly List<Consumption> _consumptions = [];
    public Guid Id { get; private set; }
    public string JobNumber { get; private set; } = string.Empty;
    public JobStatus Status { get; private set; }
    public DateTimeOffset? StartedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public IReadOnlyCollection<Consumption> Consumptions => _consumptions.AsReadOnly();
    
    public Job(Guid id, string jobNumber)
    {
        Id = id;
        JobNumber = jobNumber.Trim();
        Status = JobStatus.Draft;
    }
}
