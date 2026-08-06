namespace RadiologyCenter.Desktop.Components.Analytics;

public static class AnalyticsFormat
{
    public static string Money(decimal value) => value.ToString("#,0.##");

    public static string MoneyCompact(decimal value)
        => Math.Abs(value) >= 1_000_000 ? (value / 1_000_000m).ToString("0.##") + "M"
        : Math.Abs(value) >= 1_000 ? (value / 1_000m).ToString("0.#") + "k"
        : Money(value);

    public static string Count(int value) => value.ToString("N0");

    public static string Percent(decimal ratio) => (ratio * 100).ToString("0.#") + "%";
}