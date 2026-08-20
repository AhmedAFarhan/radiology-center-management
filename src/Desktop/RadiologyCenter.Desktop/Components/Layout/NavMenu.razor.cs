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

namespace RadiologyCenter.Desktop.Components.Layout;

public partial class NavMenu : ComponentBase, IDisposable
{
[Parameter]
    public bool IsCollapsed { get; set; }

    protected override void OnInitialized()
    {
        Permissions.ReadyChanged += OnPermissionsChanged;
    }

    private void OnPermissionsChanged() => InvokeAsync(StateHasChanged);

    private bool Can(string code) => Permissions.HasPermission(code);

    private bool CanAny(params string[] codes) => Permissions.HasAny(codes);

    private void Go(string href) => Navigation.NavigateTo(href);

    public void Dispose()
    {
        Permissions.ReadyChanged -= OnPermissionsChanged;
    }
}