using System.Globalization;

namespace RadiologyCenter.Desktop.Components.Analytics;

public static class AnalyticsFormat
{
    public static string Money(decimal value) => value.ToString("#,0.##", CultureInfo.CurrentCulture);

    public static string MoneyCompact(decimal value, string thousandSuffix = "k", string millionSuffix = "M")
        => Math.Abs(value) >= 1_000_000 ? (value / 1_000_000m).ToString("0.##", CultureInfo.CurrentCulture) + millionSuffix
        : Math.Abs(value) >= 1_000 ? (value / 1_000m).ToString("0.#", CultureInfo.CurrentCulture) + thousandSuffix
        : Money(value);

    public static string Count(int value) => value.ToString("N0", CultureInfo.CurrentCulture);

    public static string Percent(decimal ratio) => (ratio * 100).ToString("0.#", CultureInfo.CurrentCulture) + "%";
}