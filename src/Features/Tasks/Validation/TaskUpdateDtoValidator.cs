using FluentValidation;
using Rotinik.Features.Tasks.DTOs;

namespace Rotinik.Features.Tasks.Validation;

public class TaskUpdateDtoValidator : AbstractValidator<TaskUpdateDto>
{
    public TaskUpdateDtoValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(150).WithMessage("Title cannot exceed 150 characters.")
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.Frequency).IsInEnum().When(x => x.Frequency.HasValue).WithMessage("Invalid frequency.");
        RuleFor(x => x.Priority).IsInEnum().When(x => x.Priority.HasValue).WithMessage("Invalid priority.");
    }
}