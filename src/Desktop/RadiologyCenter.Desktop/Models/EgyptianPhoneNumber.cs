using System.Text.RegularExpressions;

namespace RadiologyCenter.Desktop.Models;

public static class EgyptianPhoneNumber
{
    private const string Pattern = @"^(?:01[0125][0-9]{8}|0[2-9][0-9]{7,8})$";

    public static bool IsValid(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        var normalized = Regex.Replace(phoneNumber.Trim(), @"[\s\-\(\)]", string.Empty);

        if (normalized.StartsWith("+20", StringComparison.Ordinal))
            normalized = "0" + normalized[3..];
        else if (normalized.StartsWith("0020", StringComparison.Ordinal))
            normalized = "0" + normalized[4..];

        return Regex.IsMatch(normalized, Pattern);
    }
}
