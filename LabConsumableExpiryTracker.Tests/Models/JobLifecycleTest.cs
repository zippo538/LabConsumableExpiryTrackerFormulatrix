using LabConsumableExpiryTracker.Models;
using LabConsumableExpiryTracker.Models.Enums;
using NUnit.Framework;

namespace LabConsumableExpiryTracker.Tests;

public class JobLifecycleTest
{
    private static readonly DateTimeOffset StartedAt = new(2026, 9, 9, 10, 0, 0, TimeSpan.Zero);
    [Test]
    [Category("Feature04")]
    public void Start_from_draft_sets_in_progress_and_started_at()
    {
        var job = new Job(Guid.NewGuid(), "JOB-001");
        job.Start(StartedAt);
        Assert.That(job.Status, Is.EqualTo(JobStatus.InProgress));
        Assert.That(job.StartedAt, Is.EqualTo(StartedAt));
        Assert.That(job.CompletedAt, Is.Null);
    }
    [Test]
    [Category("Feature04")]
    public void Complete_from_in_progress_sets_completed_and_completed_at()
    {
        var job = new Job(Guid.NewGuid(), "JOB-001"); job.Start(StartedAt);
        var completedAt = StartedAt.AddHours(2); job.Complete(completedAt);
        Assert.That(job.Status, Is.EqualTo(JobStatus.Completed));
        Assert.That(job.StartedAt, Is.EqualTo(StartedAt));
        Assert.That(job.CompletedAt, Is.EqualTo(completedAt));
    }
    [Test]
    [Category("Feature04")]
    public void Start_cannot_be_called_twice()
    {
        var job = new Job(Guid.NewGuid(), "JOB-001"); job.Start(StartedAt);
        Assert.That(() => job.Start(StartedAt.AddMinutes(1)), Throws.TypeOf<InvalidOperationException>());
    }
    [Test]
    [Category("Feature04")]
    public void Complete_requires_in_progress_status()
    {
        var job = new Job(Guid.NewGuid(), "JOB-001");
        Assert.That(() => job.Complete(StartedAt), Throws.TypeOf<InvalidOperationException>());
    }
}
