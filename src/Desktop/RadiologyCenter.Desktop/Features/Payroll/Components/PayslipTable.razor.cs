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

namespace RadiologyCenter.Desktop.Features.Payroll.Components;

public partial class PayslipTable : ComponentBase
{
    [Parameter] public IReadOnlyList<PayslipDto> Payslips { get; set; } = Array.Empty<PayslipDto>();
    [Parameter] public bool CanEdit { get; set; }
    [Parameter] public bool Busy { get; set; }
    [Parameter] public IReadOnlyDictionary<string, string> StaffNames { get; set; } = new Dictionary<string, string>();

    [Parameter] public EventCallback OnAddPayslip { get; set; }
    [Parameter] public EventCallback<PayslipDto> OnRemovePayslip { get; set; }
    [Parameter] public EventCallback<PayslipDto> OnExportPdf { get; set; }

    private readonly HashSet<string> _expandedPayslips = new();

    private bool IsExpanded(PayslipDto payslip) => _expandedPayslips.Contains(payslip.StaffId);

    private void ToggleExpand(PayslipDto payslip)
    {
        if (!_expandedPayslips.Add(payslip.StaffId))
            _expandedPayslips.Remove(payslip.StaffId);
    }

    private string ResolveStaff(string staffId)
        => StaffNames.TryGetValue(staffId, out var name) ? name : "-";
}
