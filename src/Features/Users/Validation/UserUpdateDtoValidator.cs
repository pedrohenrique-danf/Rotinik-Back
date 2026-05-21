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
            
        RuleFor(user => user.Password)
            .MustBeStrongPassword()
            .When(user => !string.IsNullOrEmpty(user.Password)); 
    }
}