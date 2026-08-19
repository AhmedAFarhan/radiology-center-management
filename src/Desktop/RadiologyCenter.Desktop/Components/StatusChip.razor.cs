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
using Size = MudBlazor.Size;
using Color = MudBlazor.Color;

namespace RadiologyCenter.Desktop.Components;

public partial class StatusChip : ComponentBase
{
[Parameter] public string? Token { get; set; }
    [Parameter] public Size Size { get; set; } = Size.Small;
    [Parameter] public Variant Variant { get; set; } = Variant.Filled;
    [Parameter] public Color? Color { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
}