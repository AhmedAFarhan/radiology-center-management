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

namespace RadiologyCenter.Desktop.Features.Dashboard.Pages;

public partial class RevenueChart : ComponentBase
{
[Parameter] public IReadOnlyList<RevenuePoint> Data { get; set; } = Array.Empty<RevenuePoint>();

    private static readonly ApexChartOptions<RevenuePoint> _options = new()
    {
        Chart = new ApexCharts.Chart
        {
            Toolbar = new Toolbar { Show = false },
            Zoom = new Zoom { Enabled = false },
            ForeColor = "#8B93A7",
            FontFamily = "Montserrat, Cairo, sans-serif",
        },
        DataLabels = new DataLabels { Enabled = false },
        Stroke = new Stroke { Curve = Curve.Smooth, Width = 3 },
        Colors = new List<string> { "#4C58E0" },
    };
}
