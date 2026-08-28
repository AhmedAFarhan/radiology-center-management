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

namespace RadiologyCenter.Desktop.Features.Dashboard.Pages;

public partial class DoctorCard : ComponentBase
{
[Parameter] public string Name { get; set; } = string.Empty;

    [Parameter] public string Specialty { get; set; } = string.Empty;

    [Parameter] public int Referrals { get; set; }

    private string Initials
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Name))
                return "?";

            var parts = Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var letters = parts.Take(2).Select(p => p[0]);
            return string.Concat(letters).ToUpperInvariant();
        }
    }
}
