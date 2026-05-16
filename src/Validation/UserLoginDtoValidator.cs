using FluentValidation;
using Rotinik.DTOs.User;

namespace Rotinik.Validation;

public class UserLoginDtoValidator : AbstractValidator<UserLoginDto>
{
    public UserLoginDtoValidator()
    {
        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required.");

        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}