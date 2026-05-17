using FluentValidation;
using Rotinik.DTOs.User;
using Rotinik.Validation.Extensions;

namespace Rotinik.Validation;

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