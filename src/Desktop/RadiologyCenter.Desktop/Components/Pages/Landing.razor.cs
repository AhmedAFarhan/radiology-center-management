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
using RadiologyCenter.Desktop.Components.Pages.Auth;

namespace RadiologyCenter.Desktop.Components.Pages;

public partial class Landing : ComponentBase
{
private bool _checking = true;

    protected override async Task OnInitializedAsync()
    {
        _checking = true;
        var state = await AuthState.GetAuthenticationStateAsync();
        if (state.User.Identity?.IsAuthenticated == true)
        {
            Navigation.NavigateTo("/dashboard", replace: true);
            return;
        }

        _checking = false;
    }

    private async Task OpenLogin()
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true,
            BackdropClick = true,
            NoHeader = true,
        };
        await DialogService.ShowAsync<LoginDialog>(T.Login.DialogTitle, options);
    }
}