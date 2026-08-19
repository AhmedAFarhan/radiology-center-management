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

public partial class PayRunDetailDialog : ComponentBase
{
[Parameter] public string PayRunId { get; set; } = string.Empty;

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private PayRunDto? _payRun;
    private IReadOnlyDictionary<string, string> _staffNames = new Dictionary<string, string>();
    private readonly HashSet<string> _expandedPayslips = new();
    private bool _busy;

    private bool IsExpanded(PayslipDto payslip) => _expandedPayslips.Contains(payslip.StaffId);

    private void ToggleExpand(PayslipDto payslip)
    {
        if (!_expandedPayslips.Add(payslip.StaffId))
            _expandedPayslips.Remove(payslip.StaffId);
    }

    private bool CanEditPayslips => _payRun?.Status == "Draft";
    private bool CanCompute => _payRun?.Status == "Draft";
    private bool CanApprove => _payRun?.Status == "Computed";
    private bool CanReject => _payRun?.Status == "Computed";
    private bool CanRestart => _payRun?.Status == "Rejected";
    private bool CanPay => _payRun?.Status == "Approved";

    protected override async Task OnInitializedAsync()
    {
        await SafeExecute.RunAsync(async () =>
            {
                var staffTask = ResourceService.GetStaffsPagedAsync(null, null, false, 1, 100);
                var payRunTask = PayrollService.GetPayRunByIdAsync(PayRunId);
                await Task.WhenAll(staffTask, payRunTask);

                _payRun = await payRunTask;
                _staffNames = (await staffTask).Items.ToDictionary(s => s.Id, s => s.FullName);
            },
            Snackbar,
            () => T.PayRunDialog.LoadError);
    }

    private string ResolveStaff(string staffId)
        => _staffNames.TryGetValue(staffId, out var name) ? name : staffId;

    private async Task ReloadAsync()
    {
        await SafeExecute.RunAsync(async () =>
            {
                _payRun = await PayrollService.GetPayRunByIdAsync(PayRunId);
            },
            Snackbar,
            () => T.PayRunDialog.ReloadError);
    }

    private async Task AddPayslipAsync()
    {
        var parameters = new DialogParameters { ["StaffSearchFunc"] = (Func<string, CancellationToken, Task<IEnumerable<StaffDto>>>)SearchStaffAsync };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<AddPayslipDialog>(T.Payslip.AddTitle, parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false } && result.Data is string staffId)
        {
            await SafeExecute.RunAsync(async () =>
                {
                    _busy = true;
                    await PayrollService.AddPayslipAsync(PayRunId, staffId);
                    Snackbar.Add(T.PayRunDialog.PayslipAdded, Severity.Success);
                    await ReloadAsync();
                },
                Snackbar,
                () => T.PayRunDialog.AddError,
                busy => _busy = busy);
        }
    }

    private async Task<IEnumerable<StaffDto>> SearchStaffAsync(string? value, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Array.Empty<StaffDto>();

        try
        {
            var page = await ResourceService.GetStaffsPagedAsync(value, "LastName", false, 1, 20, ct);
            return page.Items;
        }
        catch (Exception)
        {
            return Array.Empty<StaffDto>();
        }
    }

    private async Task RemovePayslipAsync(PayslipDto payslip)
    {
        var parameters = new DialogParameters
        {
            ["Title"] = T.PayRunDialog.RemovePayslipTitle,
            ["Message"] = T.FormatValue(T.PayRunDialog.RemoveConfirm, ResolveStaff(payslip.StaffId)),
            ["Icon"] = Icons.Material.Filled.Delete,
            ["Color"] = MudBlazor.Color.Error,
            ["ConfirmText"] = T.PayRunDialog.Remove,
            ["CancelText"] = T.Common.Cancel,
        };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, options);
        var result = await dialog.Result;

        if (result?.Canceled != false)
            return;

        await SafeExecute.RunAsync(async () =>
            {
                _busy = true;
                await PayrollService.RemovePayslipAsync(PayRunId, payslip.StaffId);
                Snackbar.Add(T.PayRunDialog.PayslipRemoved, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.PayRunDialog.RemoveError,
            busy => _busy = busy);
    }

    private async Task RunLifecycleAsync(string verb, string confirmTitle, string confirmMessage, string confirmOk)
    {
        var parameters = new DialogParameters
        {
            ["Title"] = confirmTitle,
            ["Message"] = confirmMessage,
            ["Icon"] = Icons.Material.Filled.HelpOutline,
            ["Color"] = MudBlazor.Color.Primary,
            ["ConfirmText"] = confirmOk,
            ["CancelText"] = T.Common.Cancel,
        };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, options);
        var result = await dialog.Result;

        if (result?.Canceled != false)
            return;

        await SafeExecute.RunAsync(async () =>
            {
                _busy = true;
                switch (verb)
                {
                    case "compute": await PayrollService.ComputePayRunAsync(PayRunId); break;
                    case "approve": await PayrollService.ApprovePayRunAsync(PayRunId); break;
                    case "reject": await PayrollService.RejectPayRunAsync(PayRunId); break;
                    case "restart": await PayrollService.RestartPayRunAsync(PayRunId); break;
                    case "pay": await PayrollService.PayPayRunAsync(PayRunId); break;
                }

                Snackbar.Add(T.FormatValue(T.PayRunDialog.LifecycleDone, verb), Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.PayRunDialog.ServerError,
            busy => _busy = busy);
    }

    private Task ComputeAsync() => RunLifecycleAsync("compute", T.PayRunDialog.ComputeTitle, T.PayRunDialog.ComputeConfirm, T.PayRunDialog.Compute);
    private Task ApproveAsync() => RunLifecycleAsync("approve", T.PayRunDialog.ApproveTitle, T.PayRunDialog.ApproveConfirm, T.PayRunDialog.Approve);
    private Task RejectAsync() => RunLifecycleAsync("reject", T.PayRunDialog.RejectTitle, T.PayRunDialog.RejectConfirm, T.PayRunDialog.Reject);
    private Task RestartAsync() => RunLifecycleAsync("restart", T.PayRunDialog.RestartTitle, T.PayRunDialog.RestartConfirm, T.PayRunDialog.Restart);
    private Task PayAsync() => RunLifecycleAsync("pay", T.PayRunDialog.PayTitle, T.PayRunDialog.PayConfirm, T.PayRunDialog.Pay);

    private void CloseAsync()
        => MudDialog.Close(DialogResult.Ok(true));
}