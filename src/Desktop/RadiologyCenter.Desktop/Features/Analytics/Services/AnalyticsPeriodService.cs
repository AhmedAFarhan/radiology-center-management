using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Features.Analytics.Services;

public enum AnalyticsRangePreset
{
    Last7Days,
    Last30Days,
    Last90Days,
    ThisMonth,
    ThisYear,
    AllTime,
}

public sealed class AnalyticsPeriodService
{
    private readonly AppLocalizer _t;
    private readonly UserTimezoneService _timezoneService;

    public AnalyticsRangePreset Selected { get; private set; } = AnalyticsRangePreset.Last30Days;

    public DateTime From { get; private set; }

    /// <summary>Exclusive upper bound (one day after the visible last day).</summary>
    public DateTime To { get; private set; }

    public string Label => Selected switch
    {
        AnalyticsRangePreset.Last7Days => _t.Analytics.Last7Days,
        AnalyticsRangePreset.Last90Days => _t.Analytics.Last90Days,
        AnalyticsRangePreset.ThisMonth => _t.Analytics.ThisMonth,
        AnalyticsRangePreset.ThisYear => _t.Analytics.ThisYear,
        AnalyticsRangePreset.AllTime => _t.Analytics.AllTime,
        _ => _t.Analytics.Last30Days,
    };

    public event Action? Changed;

    public AnalyticsPeriodService(AppLocalizer t, UserTimezoneService timezoneService)
    {
        _t = t;
        _timezoneService = timezoneService;
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
        var today = TimezoneHelper.GetToday(_timezoneService.GetTimeZoneId()).ToDateTime(TimeOnly.MinValue);

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
    }
}
