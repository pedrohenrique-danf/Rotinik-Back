using FluentValidation;
using Rotinik.Features.Tasks.DTOs;

namespace Rotinik.Features.Tasks.Validation;

public class TaskCreateDtoValidator : AbstractValidator<TaskCreateDto>
{
    public TaskCreateDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(TaskConstants.TitleMaxLength)
            .WithMessage($"Title cannot exceed {TaskConstants.TitleMaxLength} characters.");

        RuleFor(x => x.Description)
            .MaximumLength(TaskConstants.DescriptionMaxLength)
            .WithMessage($"Description cannot exceed {TaskConstants.DescriptionMaxLength} characters.");

        RuleFor(x => x.Priority).IsInEnum().WithMessage("Invalid priority.");
    }
}