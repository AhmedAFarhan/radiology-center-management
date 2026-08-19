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
using Color = MudBlazor.Color;

namespace RadiologyCenter.Desktop.Components;

public partial class ErrorState : ComponentBase
{
[Parameter] public string Icon { get; set; } = Icons.Material.Filled.ErrorOutline;

    [Parameter] public Color Color { get; set; } = Color.Error;

    [Parameter] public string? Title { get; set; }

    [Parameter] public string? Message { get; set; }

    [Parameter] public string? Detail { get; set; }

    [Parameter] public string? RetryLabel { get; set; }

    [Parameter] public EventCallback Action { get; set; }

    private string _title = string.Empty;
    private string _message = string.Empty;
    private string _retryLabel = string.Empty;

    protected override void OnParametersSet()
    {
        _title = Title ?? T.Common.ErrorTitle;
        _message = Message ?? string.Empty;
        _retryLabel = RetryLabel ?? T.Common.Retry;
    }

    private Task OnActionClick()
        => Action.HasDelegate ? Action.InvokeAsync(null) : Task.CompletedTask;
}