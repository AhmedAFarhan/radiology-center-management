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
using RadiologyCenter.Desktop.Features.Analytics.Models;
using RadiologyCenter.Desktop.Services;
using RadiologyCenter.Desktop.Features.Analytics.Components;

namespace RadiologyCenter.Desktop.Features.Analytics.Pages;

public partial class OperationalAnalytics : AnalyticsPageBase
{
[Inject]
    private AnalyticsService Api { get; set; } = null!;

    private OperationalAnalyticsDto? _data;

    protected override async Task LoadAsync(DateTime from, DateTime to)
    {
        _data = await Api.GetOperationalAsync(from, to);
    }

    private string FormatMinutes(double minutes) => minutes.ToString("0.#") + "m";

    private IReadOnlyList<AnalyticsSlice> BuildFunnel()
        => (_data?.Funnel ?? Array.Empty<StatusCountDto>())
            .Select(s => new AnalyticsSlice(s.Status, s.Count))
            .ToList();

    private IReadOnlyList<AnalyticsSlice> BuildPriority()
        => (_data?.VolumeByPriority ?? Array.Empty<PriorityVolumeDto>())
            .Select(p => new AnalyticsSlice(p.Priority, p.Count))
            .ToList();
}

