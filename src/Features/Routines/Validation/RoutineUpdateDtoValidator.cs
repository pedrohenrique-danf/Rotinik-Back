using System;
using FluentValidation;
using Rotinik.Features.Routines.DTOs;

namespace Rotinik.Features.Routines.Validation;

public class RoutineUpdateDtoValidator : AbstractValidator<RoutineUpdateDto>
{
    public RoutineUpdateDtoValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(100).WithMessage("'{PropertyName}' cannot exceed {MaxLength} characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Title));

        RuleFor(x => x.Category)
            .IsEnumName(typeof(RoutineCategory), caseSensitive: false)
            .WithMessage($"Invalid '{{PropertyName}}'. Must be one of: {string.Join(", ", Enum.GetNames(typeof(RoutineCategory)))}.")
            .When(x => !string.IsNullOrWhiteSpace(x.Category));
            
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("'{PropertyName}' cannot exceed {MaxLength} characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
            
        RuleFor(x => x.Frequency)
            .IsEnumName(typeof(RoutineFrequency), caseSensitive: false)
            .WithMessage($"Invalid '{{PropertyName}}'. Must be one of: {string.Join(", ", Enum.GetNames(typeof(RoutineFrequency)))}.")
            .When(x => !string.IsNullOrWhiteSpace(x.Frequency));
    }
}