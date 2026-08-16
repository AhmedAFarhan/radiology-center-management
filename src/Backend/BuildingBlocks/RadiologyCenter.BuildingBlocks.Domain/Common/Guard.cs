using System.Diagnostics.CodeAnalysis;
using RadiologyCenter.BuildingBlocks.Domain.Exceptions;
using RadiologyCenter.BuildingBlocks.Domain.Localization;

namespace RadiologyCenter.BuildingBlocks.Domain.Common;

public static class Guard
{
    public static T AgainstNull<T>([NotNull] T? value, string parameterName)
        where T : class
    {
        if (value is null)
            throw new DomainException(MessageCodes.Shared.CannotBeNull, $"{parameterName} cannot be null.");
        return value;
    }

    public static string AgainstNullOrWhiteSpace(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(MessageCodes.Shared.CannotBeNullOrWhitespace, $"{parameterName} cannot be null or whitespace.");
        return value;
    }

    public static int AgainstNegativeOrZero(int value, string parameterName)
    {
        if (value <= 0)
            throw new DomainException(MessageCodes.Shared.MustBeGreaterThanZero, $"{parameterName} must be greater than zero.");
        return value;
    }

    public static decimal AgainstNegativeOrZero(decimal value, string parameterName)
    {
        if (value <= 0)
            throw new DomainException(MessageCodes.Shared.MustBeGreaterThanZero, $"{parameterName} must be greater than zero.");
        return value;
    }

    public static DateTime AgainstDefault(DateTime value, string parameterName)
    {
        if (value == default)
            throw new DomainException(MessageCodes.Shared.CannotBeDefaultValue, $"{parameterName} cannot be the default value.");
        return value;
    }

    public static Guid AgainstEmpty(Guid value, string parameterName)
    {
        if (value == Guid.Empty)
            throw new DomainException(MessageCodes.Shared.CannotBeEmpty, $"{parameterName} cannot be empty.");
        return value;
    }

    public static T Against<T>(T value, Func<T, bool> predicate, string message)
    {
        if (predicate(value))
            throw new DomainException(message);
        return value;
    }

    public static T Against<T>(T value, Func<T, bool> predicate, string code, string message)
    {
        if (predicate(value))
            throw new DomainException(code, message);
        return value;
    }
}
