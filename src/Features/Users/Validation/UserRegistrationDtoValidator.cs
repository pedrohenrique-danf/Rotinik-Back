using FluentValidation;
using Rotinik.Features.Users.DTOs;
using Rotinik.Core.Extensions;

namespace Rotinik.Features.Users.Validation;

public class UserRegistrationDtoValidator : AbstractValidator<UserRegistrationDto>
{
    public UserRegistrationDtoValidator()
    {
        RuleFor(user => user.Name)
            .MustBeValidName();

        RuleFor(user => user.UserName)
            .NotEmpty().WithMessage("Username is required.")
            .MaximumLength(50).WithMessage("Username cannot exceed 50 characters.");

        RuleFor(user => user.Email)
            .MustBeValidEmail();

        RuleFor(user => user.BirthDate)
            .MustBeValidBirthDate();

        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MustBeStrongPassword();
    }
}