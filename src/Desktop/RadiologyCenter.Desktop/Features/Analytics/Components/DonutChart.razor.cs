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
using ApexCharts;

namespace RadiologyCenter.Desktop.Features.Analytics.Components;

public partial class DonutChart : ComponentBase
{
[Parameter] public IReadOnlyList<AnalyticsSlice> Items { get; set; } = Array.Empty<AnalyticsSlice>();

    [Parameter] public string Height { get; set; } = "300px";

    private static readonly ApexChartOptions<AnalyticsSlice> _options = new()
    {
        Chart = new ApexCharts.Chart
        {
            Type = ApexCharts.ChartType.Donut,
            ForeColor = "#8B93A7",
            FontFamily = "Montserrat, Cairo, sans-serif",
        },
        DataLabels = new DataLabels { Enabled = false },
        Legend = new Legend { Position = LegendPosition.Bottom },
        Stroke = new Stroke { Colors = new List<string> { "#FFFFFF" }, Width = 3 },
        Colors = new List<string>
        {
            "#4C58E0", "#34A853", "#ED6C02", "#2196F3", "#D32F2F", "#7A84EA", "#9BB3FD", "#2E7D32",
        },
    };
}
