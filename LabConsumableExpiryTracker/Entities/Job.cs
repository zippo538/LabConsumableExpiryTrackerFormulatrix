using LabConsumableExpireTracker.Domain.Common;
using LabConsumableExpireTracker.Domain.Enums;

namespace LabConsumableExpireTracker.Domain.Entities;

public sealed class Job
{
    private readonly List<Consumption> _consumptions = [];

    private Job()
    {
    }

    public Job(Guid id, string jobNumber)
    {
        if (id == Guid.Empty) throw new DomainException("Job ID is required.");
        if (string.IsNullOrWhiteSpace(jobNumber)) throw new DomainException("Job number is required.");

        Id = id;
        JobNumber = jobNumber.Trim();
        Status = JobStatus.Draft;
    }

    public Guid Id { get; private set; }
    public string JobNumber { get; private set; } = string.Empty;
    public JobStatus Status { get; private set; }
    public DateTimeOffset? StartedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public IReadOnlyCollection<Consumption> Consumptions => _consumptions.AsReadOnly();

    public void Start(DateTimeOffset now)
    {
        if (Status != JobStatus.Draft) throw new DomainException("Only a draft job can be started.");
        Status = JobStatus.InProgress;
        StartedAt = now;
    }

    public void RecordConsumption(Consumption consumption)
    {
        ArgumentNullException.ThrowIfNull(consumption);
        if (Status != JobStatus.InProgress)
            throw new DomainException("Consumption can only be recorded for an active job.");
        if (consumption.JobId != Id) throw new DomainException("Consumption belongs to another job.");

        _consumptions.Add(consumption);
    }

    public void Complete(DateTimeOffset now)
    {
        if (Status != JobStatus.InProgress) throw new DomainException("Only an active job can be completed.");
        if (StartedAt.HasValue && now < StartedAt.Value)
            throw new DomainException("Completion time cannot precede the start time.");

        Status = JobStatus.Completed;
        CompletedAt = now;
    }

    public void Cancel()
    {
        if (Status is JobStatus.Completed or JobStatus.Cancelled)
            throw new DomainException("The job is already closed.");

        Status = JobStatus.Cancelled;
    }
}
