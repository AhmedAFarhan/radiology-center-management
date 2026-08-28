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

namespace RadiologyCenter.Desktop.Features.Dashboard.Components;

public partial class StatCard : ComponentBase
{
[Parameter] public string Label { get; set; } = string.Empty;
    [Parameter] public string Value { get; set; } = string.Empty;
    [Parameter] public string Icon { get; set; } = string.Empty;
    [Parameter] public Color Color { get; set; } = Color.Primary;
    [Parameter] public string? TrendText { get; set; }
    [Parameter] public bool TrendUp { get; set; } = true;
    [Parameter] public RenderFragment? Trend { get; set; }
}

