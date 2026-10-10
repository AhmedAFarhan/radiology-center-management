using FluentValidation;

namespace RadiologyCenter.BuildingBlocks.Application.Validation;

public static class PasswordPolicyRule
{
    public const int MinLength = 12;
    public const int MaxLength = 128;

    public static bool IsStrongPassword(string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        if (password.Length < MinLength || password.Length > MaxLength)
            return false;

        return password.Any(c => c is >= 'A' and <= 'Z') && password.Any(c => c is >= 'a' and <= 'z')
            && password.Any(c => c is >= '0' and <= '9') && password.Any(c => !char.IsAsciiLetterOrDigit(c));
    }

    public static IRuleBuilderOptions<T, string?> StrongPassword<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        string errorCode = "Shared.PasswordPolicy") =>
        ruleBuilder.Must(IsStrongPassword)
            .WithMessage($"Password must be {MinLength}–{MaxLength} characters long and contain uppercase, lowercase, a number and a symbol.")
            .WithErrorCode(errorCode);
}
