using FluentValidation;
using Rotinik.DTOs.User;

namespace Rotinik.Validation;

public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
{
    public UserUpdateDtoValidator()
    {
        RuleFor(user => user.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(user => user.BirthDate)
            .NotEmpty().WithMessage("Birth date is required.")
            .LessThan(DateTime.UtcNow).WithMessage("Birth date cannot be in the future.");
            
        RuleFor(user => user.Password)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$")
            .WithMessage("Password requirements: minimum 8 characters, 1 uppercase, 1 lowercase, 1 number, and 1 symbol.")
            .When(user => !string.IsNullOrEmpty(user.Password)); 
    }
}