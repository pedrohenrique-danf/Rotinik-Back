using System;
using FluentValidation;
using Rotinik.Features.Users.DTOs;
using Rotinik.Core.Extensions;

namespace Rotinik.Features.Users.Validation;

public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
{
    public UserUpdateDtoValidator()
    {
        RuleFor(user => user.Name)
            .MustBeValidName();

        RuleFor(user => user.BirthDate)
            .LessThanOrEqualTo(x => DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Birth date cannot be in the future.")
            .GreaterThanOrEqualTo(x => DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-150))
            .WithMessage("Birth date cannot be more than 150 years ago.");
            
        RuleFor(user => user.Password)
            .MustBeStrongPassword()
            .When(user => !string.IsNullOrEmpty(user.Password)); 
    }
}