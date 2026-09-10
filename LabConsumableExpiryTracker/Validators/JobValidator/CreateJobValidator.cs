using FluentValidation;
using LabConsumableExpiryTracker.DTOs.JobDTOs;
using LabConsumableExpiryTracker.Validators.Helpers;

namespace LabConsumableExpiryTracker.Validators.JobValidator;

public sealed class CreateJobValidator : AbstractValidator<CreateJobDto>
{
    public CreateJobValidator()
    {
        RuleFor(x => x.JobNumber)
            .Cascade(CascadeMode.Stop)
            .Must(LotHelperValidator.NotBeBlank)
            .WithMessage("JobNumber wajib diisi.")
            .MaximumLength(100)
            .WithMessage("JobNumber maksimal 100 karakter.");
    }
}
