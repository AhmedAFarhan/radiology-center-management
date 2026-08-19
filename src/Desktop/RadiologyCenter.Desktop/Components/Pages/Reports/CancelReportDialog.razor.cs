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

namespace RadiologyCenter.Desktop.Components.Pages.Reports;

public partial class CancelReportDialog : ComponentBase
{
[Parameter] public string ReportId { get; set; } = string.Empty;

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private string? _reason;
    private bool _busy;

    private async Task SubmitAsync()
    {
        await SafeExecute.RunAsync(async () =>
            {
                await ReportService.CancelAsync(ReportId, _reason);
                Snackbar.Add(T.CancelReport.Cancelled, Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            },
            Snackbar,
            () => T.CancelReport.Unreachable,
            busy => _busy = busy);
    }

    private void CancelAsync()
        => MudDialog.Cancel();
}