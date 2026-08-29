using FluentValidation;
using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.BuildingBlocks.Application.Validation;

public static class EnumerationRuleExtensions
{
    public static IRuleBuilderOptions<T, string> IsEnumerationMember<TEnum, T>(
        this IRuleBuilder<T, string> ruleBuilder,
        string label,
        string errorCode = "Shared.InvalidEnumValue")
        where TEnum : Enumeration
    {
        var names = Enumeration.GetAll<TEnum>()
            .Select(e => e.Name)
            .ToList();

        return ruleBuilder.Must(name => names.Any(n => n.Equals(name, StringComparison.OrdinalIgnoreCase)))
            .WithMessage($"{label} must be one of: {string.Join(", ", names)}.")
            .WithErrorCode(errorCode);
    }

    public static IRuleBuilderOptions<T, string?> IsEnumerationMemberOrEmpty<TEnum, T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        string label,
        string errorCode = "Shared.InvalidEnumValue")
        where TEnum : Enumeration
    {
        var names = Enumeration.GetAll<TEnum>()
            .Select(e => e.Name)
            .ToList();

        return ruleBuilder
            .Must(name => string.IsNullOrWhiteSpace(name) || names.Any(n => n.Equals(name, StringComparison.OrdinalIgnoreCase)))
            .WithMessage($"{label} must be one of: {string.Join(", ", names)}.")
            .WithErrorCode(errorCode);
    }
}
