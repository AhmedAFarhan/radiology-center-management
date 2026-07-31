using System.Text.RegularExpressions;
using FluentValidation;

namespace RadiologyCenter.BuildingBlocks.Application.Validation;

public static class EgyptianPhoneNumberRule
{
    private const string Pattern = @"^(?:01[0125][0-9]{8}|0[2-9][0-9]{7,8})$";

    private static readonly Regex PhoneRegex = new(
        Pattern,
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static bool IsValidEgyptianPhoneNumber(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        var normalized = Regex.Replace(phoneNumber.Trim(), @"[\s\-\(\)]", string.Empty);

        if (normalized.StartsWith("+20", StringComparison.Ordinal))
            normalized = "0" + normalized[3..];
        else if (normalized.StartsWith("0020", StringComparison.Ordinal))
            normalized = "0" + normalized[4..];

        return PhoneRegex.IsMatch(normalized);
    }

    public static IRuleBuilderOptions<T, string?> IsEgyptianPhoneNumber<T>(this IRuleBuilder<T, string?> ruleBuilder) =>
        ruleBuilder.Must(IsValidEgyptianPhoneNumber)
            .WithMessage("Phone number must be a valid Egyptian number (e.g. 01012345678).");
}
