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

public partial class PayRunDetailDialog : ComponentBase
{
    [Parameter] public string PayRunId { get; set; } = string.Empty;

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private PayRunDto? _payRun;
    private IReadOnlyDictionary<string, string> _staffNames = new Dictionary<string, string>();
    private IReadOnlyDictionary<string, string> _doctorNames = new Dictionary<string, string>();
    private bool _busy;

    private bool CanEditPayslips => _payRun?.StatusKey == "Draft";
    private bool CanCompute => _payRun?.StatusKey == "Draft";
    private bool CanApprove => _payRun?.StatusKey == "Computed";
    private bool CanReject => _payRun?.StatusKey == "Computed";
    private bool CanRestart => _payRun?.StatusKey == "Rejected";
    private bool CanPay => _payRun?.StatusKey == "Approved";

    protected override async Task OnInitializedAsync()
    {
        await SafeExecute.RunAsync(async () =>
            {
                var staffTask = ResourceService.GetStaffsPagedAsync(null, null, false, 1, 100);
                var payRunTask = PayrollService.GetPayRunByIdAsync(PayRunId);
                var doctorsTask = ResourceService.GetReferralDoctorsPagedAsync(null, null, false, 1, 100);
                await Task.WhenAll(staffTask, payRunTask, doctorsTask);

                _payRun = await payRunTask;
                _staffNames = (await staffTask).Items.ToDictionary(s => s.Id, s => s.FullName);
                _doctorNames = (await doctorsTask).Items.ToDictionary(d => d.Id, d => d.FullName);
            },
            Snackbar,
            () => T.PayRunDialog.LoadError);
    }

    private string ResolveStaff(string staffId)
        => _staffNames.TryGetValue(staffId, out var name) ? name : "-";

    private string ResolveDoctor(string doctorId)
        => _doctorNames.TryGetValue(doctorId, out var name) ? name : "-";

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

    private async Task ExportPayslipPdfAsync(PayslipDto payslip)
    {
        await SafeExecute.RunAsync(async () =>
            {
                _busy = true;
                var pdfBytes = await PayrollService.GetPayslipPdfAsync(PayRunId, payslip.StaffId);
                var fileName = $"payslip-{ResolveStaff(payslip.StaffId).Replace(" ", "_")}-{DateTime.Now:yyyyMMdd}.pdf";
                var path = await FileSaveHelper.SaveAsync(pdfBytes, fileName);
                Snackbar.Add(T.FormatValue(T.PayRunDialog.PayslipPdfSaved, path), Severity.Success);
            },
            Snackbar,
            () => T.PayRunDialog.PayslipPdfError,
            busy => _busy = busy);
    }

    private async Task ExportReferralStatementPdfAsync(ReferralFeeStatementDto statement)
    {
        await SafeExecute.RunAsync(async () =>
            {
                _busy = true;
                var pdfBytes = await PayrollService.GetReferralStatementPdfAsync(PayRunId, statement.ReferralDoctorId);
                var fileName = $"referral-statement-{ResolveDoctor(statement.ReferralDoctorId).Replace(" ", "_")}-{DateTime.Now:yyyyMMdd}.pdf";
                var path = await FileSaveHelper.SaveAsync(pdfBytes, fileName);
                Snackbar.Add(T.FormatValue(T.PayRunDialog.ReferralPdfSaved, path), Severity.Success);
            },
            Snackbar,
            () => T.PayRunDialog.ReferralPdfError,
            busy => _busy = busy);
    }

    private void CloseAsync()
        => MudDialog.Close(DialogResult.Ok(true));
}
