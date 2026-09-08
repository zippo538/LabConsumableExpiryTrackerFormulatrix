using FluentValidation;
using LabConsumableExpiryTracker.DTOs;
using LabConsumableExpiryTracker.Models.Enums;

<<<<<<<< HEAD:LabConsumableExpiryTracker/Validators/Item/UpdateItemValidator.cs
namespace LabConsumableExpiryTracker.Validators.Item
========
namespace LabConsumableExpiryTracker.Validators.ItemValidator
>>>>>>>> feat-masterdata:LabConsumableExpiryTracker/Validators/ItemValidator/UpdateItemValidator.cs
{
    public class UpdateItemValidator : AbstractValidator<UpdateItemDto>
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