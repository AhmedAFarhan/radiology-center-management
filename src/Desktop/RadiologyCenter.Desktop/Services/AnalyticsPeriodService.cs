namespace RadiologyCenter.Desktop.Services;

public enum AnalyticsRangePreset
{
    Last7Days,
    Last30Days,
    Last90Days,
    ThisMonth,
    ThisYear,
    AllTime,
}

public sealed record AnalyticsRange(AnalyticsRangePreset Preset, DateTime From, DateTime To, string Label);

public sealed class AnalyticsPeriodService
{
    public AnalyticsRangePreset Selected { get; private set; } = AnalyticsRangePreset.Last30Days;

    public DateTime From { get; private set; }

    /// <summary>Exclusive upper bound (one day after the visible last day).</summary>
    public DateTime To { get; private set; }

    public string Label { get; private set; } = string.Empty;

    public event Action? Changed;

    public AnalyticsPeriodService()
    {
        Apply(AnalyticsRangePreset.Last30Days);
    }

    public void SetPreset(AnalyticsRangePreset preset)
    {
        if (preset == Selected)
            return;
        Apply(preset);
        Changed?.Invoke();
    }

    public void Refresh() => Changed?.Invoke();

    private void Apply(AnalyticsRangePreset preset)
    {
        Selected = preset;
        var today = DateTime.Today;

        var from = preset switch
        {
            AnalyticsRangePreset.Last7Days => today.AddDays(-6),
            AnalyticsRangePreset.Last90Days => today.AddDays(-89),
            AnalyticsRangePreset.ThisMonth => new DateTime(today.Year, today.Month, 1),
            AnalyticsRangePreset.ThisYear => new DateTime(today.Year, 1, 1),
            AnalyticsRangePreset.AllTime => DateTime.MinValue,
            _ => today.AddDays(-29),
        };

        From = from;
        To = today.AddDays(1);

        Label = preset switch
        {
            AnalyticsRangePreset.Last7Days => "Last 7 days",
            AnalyticsRangePreset.Last30Days => "Last 30 days",
            AnalyticsRangePreset.Last90Days => "Last 90 days",
            AnalyticsRangePreset.ThisMonth => "This month",
            AnalyticsRangePreset.ThisYear => "This year",
            AnalyticsRangePreset.AllTime => "All time",
            _ => "Last 30 days",
        };
    }
}