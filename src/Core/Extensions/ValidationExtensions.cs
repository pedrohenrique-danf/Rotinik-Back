using FluentValidation;

namespace Rotinik.Core.Extensions;

public static class ValidationExtensions
{
    public static IRuleBuilderOptions<T, string> MustBeValidName<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
    }

    public static IRuleBuilderOptions<T, string> MustBeValidEmail<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required.");
    }

    public static IRuleBuilderOptions<T, DateTime> MustBeValidBirthDate<T>(this IRuleBuilder<T, DateTime> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Birth date is required.")
            .LessThan(DateTime.UtcNow).WithMessage("Birth date cannot be in the future.");
    }

    public static IRuleBuilderOptions<T, string> MustBeStrongPassword<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$")
            .WithMessage("Password requirements: minimum 8 characters, 1 uppercase, 1 lowercase, 1 number, and 1 symbol.");
    }

    public static IRuleBuilderOptions<T, string> MustBeValidPhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Phone number is required.")
            .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters.");
    }
}