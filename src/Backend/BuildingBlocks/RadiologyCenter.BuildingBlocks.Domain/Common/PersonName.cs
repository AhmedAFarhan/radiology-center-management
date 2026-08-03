using RadiologyCenter.BuildingBlocks.Domain.Exceptions;

namespace RadiologyCenter.BuildingBlocks.Domain.Common;

public static class PersonName
{
    public static (string FirstName, string? MiddleName, string LastName) Split(string fullName)
    {
        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length < 2)
            throw new DomainException("Full name must contain at least a first name and a last name.");

        var firstName = parts[0];
        var lastName = parts[^1];
        var middleName = parts.Length > 2 ? string.Join(' ', parts[1..^1]) : null;

        return (firstName, middleName, lastName);
    }

    public static bool ContainsAtLeastTwoTokens(string? fullName)
    {
        var parts = fullName?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts is { Length: >= 2 };
    }
}
