namespace RadiologyCenter.Desktop.Services;

public static class Formatting
{
    public static string Truncate(string value, int maxLength = 60)
        => value.Length > maxLength ? value[..maxLength] + "…" : value;

    public static string FormatFrequency(AppLocalizer localizer, string frequency) => frequency switch
    {
        "OneTime" => localizer.Common.FrequencyOneTime,
        "Monthly" => localizer.Common.FrequencyMonthly,
        "Quarterly" => localizer.Common.FrequencyQuarterly,
        "Annual" => localizer.Common.FrequencyAnnual,
        _ => frequency,
    };
}