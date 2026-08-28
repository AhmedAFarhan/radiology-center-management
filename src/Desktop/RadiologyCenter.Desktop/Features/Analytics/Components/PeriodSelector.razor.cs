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

namespace RadiologyCenter.Desktop.Features.Analytics.Components;

public partial class PeriodSelector : ComponentBase
{
[Parameter] public string? Class { get; set; }

    protected override void OnInitialized()
    {
        Preset.Changed += OnPeriodChanged;
    }

    private void OnPeriodChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        Preset.Changed -= OnPeriodChanged;
    }

    private Task OnPreset(AnalyticsRangePreset preset)
    {
        Preset.SetPreset(preset);
        return Task.CompletedTask;
    }
}
