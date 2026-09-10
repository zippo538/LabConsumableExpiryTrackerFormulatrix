using FluentValidation;
using LabConsumableExpiryTracker.DTOs.JobDTOs;
using LabConsumableExpiryTracker.Models.Enums;

namespace LabConsumableExpiryTracker.Validators.JobValidator;

public sealed class UpdateJobValidator : AbstractValidator<UpdateJobDto>
{
    public UpdateJobValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status Job tidak valid.")
            .Must(status => status is JobStatus.InProgress or JobStatus.Completed)
            .WithMessage("Status hanya boleh InProgress atau Completed.");
    }
}
