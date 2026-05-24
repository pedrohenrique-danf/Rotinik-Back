using FluentValidation;
using Rotinik.Features.Users.DTOs;
using Rotinik.Core.Extensions;

namespace Rotinik.Features.Users.Validation;

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