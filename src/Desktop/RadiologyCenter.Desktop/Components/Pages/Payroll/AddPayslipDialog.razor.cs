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

namespace RadiologyCenter.Desktop.Components.Pages.Payroll;

public partial class AddPayslipDialog : ComponentBase
{
[Parameter] public Func<string?, CancellationToken, Task<IEnumerable<StaffDto>>> StaffSearchFunc { get; set; } = default!;

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private StaffDto? _selectedStaff;

    private void SubmitAsync()
    {
        if (_selectedStaff is null)
        {
            Snackbar.Add(T.Payslip.SelectEmployee, Severity.Warning);
            return;
        }

        MudDialog.Close(DialogResult.Ok(_selectedStaff.Id));
    }

    private void CancelAsync()
        => MudDialog.Cancel();
}