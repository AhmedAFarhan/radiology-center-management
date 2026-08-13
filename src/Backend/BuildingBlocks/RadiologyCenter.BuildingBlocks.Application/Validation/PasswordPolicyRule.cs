using FluentValidation;

namespace RadiologyCenter.BuildingBlocks.Application.Validation;

public static class PasswordPolicyRule
{
    public const int MinLength = 8;
    public const int MaxLength = 100;

    public static bool IsStrongPassword(string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        if (password.Length < MinLength || password.Length > MaxLength)
            return false;

        return password.Any(char.IsLetter) && password.Any(char.IsDigit);
    }

    public static IRuleBuilderOptions<T, string?> StrongPassword<T>(this IRuleBuilder<T, string?> ruleBuilder) =>
        ruleBuilder.Must(IsStrongPassword)
            .WithMessage($"Password must be at least {MinLength} characters long and contain both letters and digits.");
}