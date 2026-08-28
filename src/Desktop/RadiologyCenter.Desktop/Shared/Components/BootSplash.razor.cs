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
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Shared.Components;

public partial class BootSplash : ComponentBase
{
private bool _retrying;

    protected override async Task OnInitializedAsync()
    {
        Backend.StateChanged += NotifyStateChanged;
        await Backend.EnsureStartedAsync();
    }

    private void NotifyStateChanged() => InvokeAsync(StateHasChanged);

    private async Task RetryAsync()
    {
        _retrying = true;
        StateHasChanged();
        await Backend.RetryAsync();
        _retrying = false;
    }

    public void Dispose() => Backend.StateChanged -= NotifyStateChanged;
}
