using FluentValidation;
using Rotinik.Features.Users.DTOs;
using Rotinik.Core.Validation;

namespace Rotinik.Features.Users.Validation;

public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
{
    public UserUpdateDtoValidator()
    {
        RuleFor(user => user.Name)
            .MustBeValidName();

        RuleFor(user => user.BirthDate)
            .MustBeValidBirthDate();

        RuleFor(user => user.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters.");
            
        RuleFor(user => user.Password)
            .MustBeStrongPassword()
            .When(user => !string.IsNullOrEmpty(user.Password)); 
    }
}