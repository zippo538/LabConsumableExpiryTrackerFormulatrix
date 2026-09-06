using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using LabConsumableExpiryTracker.Models.Enums;
using LabConsumableExpiryTracker.DTOs;

namespace LabConsumableExpiryTracker.Validators
{
    public class CreateItemValidator : AbstractValidator<CreateItemDTO>
    {
        public CreateItemValidator()
        {
            RuleFor(item => item.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Item code is required.")
                .MaximumLength(50).WithMessage("Item code cannot exceed 50 characters.");

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