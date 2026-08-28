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

namespace RadiologyCenter.Desktop.Features.Payroll.Components;

public partial class ReferralFeeTable : ComponentBase
{
    [Parameter] public IReadOnlyList<ReferralFeeStatementDto> Statements { get; set; } = Array.Empty<ReferralFeeStatementDto>();
    [Parameter] public IReadOnlyDictionary<string, string> DoctorNames { get; set; } = new Dictionary<string, string>();
    [Parameter] public bool Busy { get; set; }

    [Parameter] public EventCallback<ReferralFeeStatementDto> OnExportPdf { get; set; }

    private string ResolveDoctor(string doctorId)
        => DoctorNames.TryGetValue(doctorId, out var name) ? name : "-";
}
