using FluentValidation;

namespace Rotinik.Features.Routines.DTO;

public class RoutineCreateDtoValidator : AbstractValidator<RoutineCreateDto>
{
    public RoutineCreateDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required.")
            .MaximumLength(50).WithMessage("Category cannot exceed 50 characters.");
    }
}