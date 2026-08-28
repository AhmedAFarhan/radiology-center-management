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
using ApexCharts;
using Grid = ApexCharts.Grid;
using Toolbar = ApexCharts.Toolbar;

namespace RadiologyCenter.Desktop.Features.Analytics.Components;

public partial class ListBarChart : ComponentBase
{
[Parameter] public IReadOnlyList<AnalyticsSlice> Items { get; set; } = Array.Empty<AnalyticsSlice>();

    [Parameter] public string Height { get; set; } = "320px";

    [Parameter] public string SeriesName { get; set; } = "Count";

    private static readonly ApexChartOptions<AnalyticsSlice> _options = new()
    {
        Chart = new ApexCharts.Chart
        {
            Toolbar = new Toolbar { Show = false },
            ForeColor = "#8B93A7",
            FontFamily = "Montserrat, Cairo, sans-serif",
        },
        DataLabels = new DataLabels { Enabled = false },
        PlotOptions = new PlotOptions
        {
            Bar = new PlotOptionsBar { Horizontal = true, BorderRadius = 6 }
        },
        Grid = new Grid { Show = true, BorderColor = "#EDEFF5" },
        Colors = new List<string> { "#4C58E0" },
    };
}
