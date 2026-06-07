using FluentValidation;
using Rotinik.Features.Tasks.DTOs;

namespace Rotinik.Features.Tasks.Validation;

public class TaskUpdateDtoValidator : AbstractValidator<TaskUpdateDto>
{
    public TaskUpdateDtoValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(TaskConstants.TitleMaxLength)
            .WithMessage($"Title cannot exceed {TaskConstants.TitleMaxLength} characters.")
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters.")
            .When(x => x.Description != null);

        RuleFor(x => x.Frequency).IsInEnum().When(x => x.Frequency.HasValue).WithMessage("Invalid frequency.");
        RuleFor(x => x.Importance)
            .Must(x => new[] { "baixa", "media", "alta", "critica" }.Contains(x?.ToLower()))
            .When(x => !string.IsNullOrEmpty(x.Importance))
            .WithMessage("Invalid importance.");
    }
}