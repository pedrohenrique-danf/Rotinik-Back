using FluentValidation;
using Rotinik.DTOs.User;
using Rotinik.Validation.Extensions;

namespace Rotinik.Validation;

public class UserLoginDtoValidator : AbstractValidator<UserLoginDto>
{
    public UserLoginDtoValidator()
    {
        RuleFor(user => user.Email)
            .MustBeValidEmail();

        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}