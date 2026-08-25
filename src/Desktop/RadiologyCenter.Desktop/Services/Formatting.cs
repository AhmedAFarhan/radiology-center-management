namespace RadiologyCenter.Desktop.Services;

public static class Formatting
{
    public static string Truncate(string value, int maxLength = 60)
        => value.Length > maxLength ? value[..maxLength] + "…" : value;

    public static string FormatFrequency(string frequency) => frequency switch
    {
        "OneTime" => "One Time",
        "Monthly" => "Monthly",
        "Quarterly" => "Quarterly",
        "Annual" => "Annual",
        _ => frequency,
    };
}