using FluentValidation;
using Rotinik.Features.Routines.DTOs;

namespace Rotinik.Features.Routines.Validation;

public class RoutineUpdateDtoValidator : AbstractValidator<RoutineUpdateDto>
{
    public RoutineUpdateDtoValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Title));

        RuleFor(x => x.Category)
            .MaximumLength(50).WithMessage("Category cannot exceed 50 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Category));
    }
}