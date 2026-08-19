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
using Grid = ApexCharts.Grid;
using Toolbar = ApexCharts.Toolbar;

namespace RadiologyCenter.Desktop.Components.Analytics;

public partial class ModalityRevenueChart : ComponentBase
{
[Parameter] public IReadOnlyList<RevenueByModalityDto> Items { get; set; } = Array.Empty<RevenueByModalityDto>();

    [Parameter] public string Height { get; set; } = "300px";

    private static readonly ApexChartOptions<RevenueByModalityDto> _options = new()
    {
        Chart = new ApexCharts.Chart
        {
            Toolbar = new Toolbar { Show = false },
            ForeColor = "#8B93A7",
            FontFamily = "Montserrat, Cairo, sans-serif",
        },
        DataLabels = new DataLabels { Enabled = false },
        Grid = new Grid { Show = true, BorderColor = "#EDEFF5" },
        Colors = new List<string> { "#7A84EA" },
        PlotOptions = new PlotOptions
        {
            Bar = new PlotOptionsBar { BorderRadius = 8, ColumnWidth = "45%" }
        },
    };
}