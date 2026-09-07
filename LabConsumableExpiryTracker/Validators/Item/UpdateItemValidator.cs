using FluentValidation;
using LabConsumableExpiryTracker.DTOs;
using LabConsumableExpiryTracker.Models.Enums;

namespace LabConsumableExpiryTracker.Validators.Item
{
    public class UpdateItemValidator : AbstractValidator<UpdateItemDTO>
    {
        public UpdateItemValidator()
        {
            RuleFor(item => item.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Item name is required.")
            .MaximumLength(200).WithMessage("Item name cannot exceed 200 characters.");

            RuleFor(item => item.BaseUnit)
                .Must(unit => Enum.IsDefined(typeof(UnitOfMeasure), unit))
                .WithMessage("Base unit is invalid.");

            RuleFor(item => item.MinimumStock)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Minimum stock cannot be negative.");

            RuleFor(item => item.ExpiringSoonDays)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Expiring soon days cannot be negative.");
        }
    }
}