using FluentValidation;

namespace Rotinik.Features.Tasks.DTO;

public class TaskCreateDtoValidator : AbstractValidator<TaskCreateDto>
{
    public TaskCreateDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(150).WithMessage("Title cannot exceed 150 characters.");

        RuleFor(x => x.Frequency).IsInEnum().WithMessage("Invalid frequency.");
        RuleFor(x => x.Priority).IsInEnum().WithMessage("Invalid priority.");
    }
}