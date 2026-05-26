using System;
using FluentValidation;
using Rotinik.Features.Routines.DTOs;

namespace Rotinik.Features.Routines.Validation;

public class RoutineCreateDtoValidator : AbstractValidator<RoutineCreateDto>
{
    public RoutineCreateDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("'{PropertyName}' is required.")
            .MaximumLength(100).WithMessage("'{PropertyName}' cannot exceed {MaxLength} characters.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("'{PropertyName}' is required.")
            .IsEnumName(typeof(RoutineCategory), caseSensitive: false)
            .WithMessage($"Invalid '{{PropertyName}}'. Must be one of: {string.Join(", ", Enum.GetNames(typeof(RoutineCategory)))}.");
            
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("'{PropertyName}' cannot exceed {MaxLength} characters.");
            
        RuleFor(x => x.Frequency)
            .NotEmpty().WithMessage("'{PropertyName}' is required.")
            .IsEnumName(typeof(RoutineFrequency), caseSensitive: false)
            .WithMessage($"Invalid '{{PropertyName}}'. Must be one of: {string.Join(", ", Enum.GetNames(typeof(RoutineFrequency)))}.");
    }
}