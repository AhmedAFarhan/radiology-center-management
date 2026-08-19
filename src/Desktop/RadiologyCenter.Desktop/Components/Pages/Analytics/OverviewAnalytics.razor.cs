using System.Net.Http;
using System.Net.Http.Json;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using MudBlazor;
using RadiologyCenter.Desktop;
using RadiologyCenter.Desktop.Components;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;
using RadiologyCenter.Desktop.Components.Analytics;

namespace RadiologyCenter.Desktop.Components.Pages.Analytics;

public partial class OverviewAnalytics : AnalyticsPageBase
{
[Inject]
    private AnalyticsService Api { get; set; } = null!;

    private ProfitAnalyticsDto? _profit;
    private FinancialAnalyticsDto? _financial;

    protected override async Task LoadAsync(DateTime from, DateTime to)
    {
        _profit = await Api.GetProfitAsync(from, to);
        _financial = await Api.GetFinancialAsync(from, to);
    }

    private string ExamCountLabel() => T.FormatValue(T.Overview.ExamCount, AnalyticsFormat.Count(_financial!.ExamCount));

    private IReadOnlyList<AnalyticsSlice> BuildCostBreakdown()
    {
        var list = new List<AnalyticsSlice>();
        Add(list, T.Overview.StaffCaseFees, _profit!.StaffCaseFees);
        Add(list, T.Overview.ReferralFees, _profit.ReferralFees);
        Add(list, T.Overview.LaborPayroll, _profit.LaborCosts);
        Add(list, T.Overview.Materials, _profit.MaterialCosts);
        return list.Count == 0
            ? new List<AnalyticsSlice> { new AnalyticsSlice(T.Overview.NoCosts, 0) }
            : list;
    }

    private void Add(List<AnalyticsSlice> list, string label, decimal value)
    {
        if (value > 0)
            list.Add(new AnalyticsSlice(label, value));
    }

    private IReadOnlyList<AnalyticsSlice> BuildReceivableSlices()
        => _financial!.ReceivableAging
            .Select(r => new AnalyticsSlice(r.Bucket, r.Amount))
            .ToList();

    private List<WaterfallRow> BuildWaterfall()
    {
        var baseValue = _profit!.RevenueCollected;
        int percent(decimal v) => (int)Math.Round(v / (baseValue == 0 ? 1 : baseValue) * 100);

        return new List<WaterfallRow>
        {
            Build(T.Overview.Revenue, _profit.RevenueCollected, percent(_profit.RevenueCollected), "#4C58E0"),
            Build(T.Overview.StaffFees, -_profit.StaffCaseFees, percent(_profit.StaffCaseFees), "#FF8A65"),
            Build(T.Overview.ReferralFees, -_profit.ReferralFees, percent(_profit.ReferralFees), "#FF8A65"),
            Build(T.Overview.Labor, -_profit.LaborCosts, percent(_profit.LaborCosts), "#FF8A65"),
            Build(T.Overview.Materials, -_profit.MaterialCosts, percent(_profit.MaterialCosts), "#FF8A65"),
            Build(T.Overview.NetProfit, _profit.NetProfit, percent(_profit.NetProfit), "#66BB6A"),
        };
    }

    private WaterfallRow Build(string label, decimal value, int percent, string color)
        => new WaterfallRow(label, value, Math.Clamp(percent, 0, 100), color);

    private sealed record WaterfallRow(string Label, decimal Value, int Percent, string Color);
}