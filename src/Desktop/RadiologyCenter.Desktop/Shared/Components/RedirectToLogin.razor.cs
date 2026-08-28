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

namespace RadiologyCenter.Desktop.Shared.Components;

public partial class RedirectToLogin : ComponentBase
{
protected override void OnInitialized()
    {
        var relative = Navigation.ToBaseRelativePath(Navigation.Uri);
        if (!string.IsNullOrWhiteSpace(relative) && !relative.Equals("/", StringComparison.Ordinal))
            ReturnUrl.Store(Navigation.Uri);
        Navigation.NavigateTo("/", forceLoad: false);
    }
}
