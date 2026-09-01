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
using RadiologyCenter.Desktop.Features.Analytics.Models;
using RadiologyCenter.Desktop.Services;
using RadiologyCenter.Desktop.Features.Analytics.Components;

namespace RadiologyCenter.Desktop.Features.Analytics.Pages;

public partial class FinancialAnalytics : AnalyticsPageBase
{
[Inject]
    private AnalyticsService Api { get; set; } = null!;

    [Inject]
    private InsuranceService Insurance { get; set; } = null!;

    [Inject]
    private IJSRuntime JS { get; set; } = null!;

    private FinancialAnalyticsDto? _data;
    private IReadOnlyList<FinancialExamRowDto>? _exams;
    private InsuranceStatsDto? _insuranceStats;
    private bool _exporting;

    protected override async Task LoadAsync(DateTime from, DateTime to)
    {
        _data = await Api.GetFinancialAsync(from, to);
        _exams = await Api.GetFinancialExamsAsync(from, to);

        try
        {
            _insuranceStats = await Insurance.GetStatsAsync();
        }
        catch
        {
            _insuranceStats = null;
        }
    }

    private IReadOnlyList<AnalyticsSlice> BuildReceivableSlices()
        => (_data?.ReceivableAging ?? Array.Empty<ReceivableBucketDto>())
            .Select(r => new AnalyticsSlice(r.Bucket, r.Amount))
            .ToList();

    private string CollectionRate()
        => _data!.TotalBilled > 0
            ? AnalyticsFormat.Percent(_data.TotalCollected / _data.TotalBilled)
            : "-";

    private async Task ExportAsync(string format)
    {
        _exporting = true;
        StateHasChanged();
        try
        {
            var bytes = await Api.ExportAsync("financial", Period.From, Period.To, format);
            var fileName = $"FinancialReport_{Period.From:yyyyMMdd}-{Period.To:yyyyMMdd}.{format.ToLowerInvariant()}";
            var contentType = format == "Pdf" ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            await JS.InvokeVoidAsync("downloadFile", fileName, contentType, bytes);
        }
        finally
        {
            _exporting = false;
        }
    }
}

