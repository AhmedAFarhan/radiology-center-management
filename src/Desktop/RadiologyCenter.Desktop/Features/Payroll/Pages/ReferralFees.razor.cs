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

namespace RadiologyCenter.Desktop.Features.Payroll.Pages;

public partial class ReferralFees : ListPageBase<ReferralFeeDto>
{
    private IReadOnlyDictionary<string, string> _doctorNames = new Dictionary<string, string>();
    private IReadOnlyDictionary<string, string> _examTypeNames = new Dictionary<string, string>();

    protected override string UnreachableMessage => T.ReferralFee.Unreachable;

    protected override async Task<PagedResult<ReferralFeeDto>> LoadPageAsync(
        string? search,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        var feesTask = PayrollService.GetReferralFeesPagedAsync(search, sortBy, sortDescending, page, pageSize, ct);
        var doctorsTask = ResourceService.GetReferralDoctorsPagedAsync(null, null, false, 1, 100, ct);
        var typesTask = ExaminationService.GetTypesPagedAsync(null, null, false, 1, 100, ct);

        await Task.WhenAll(feesTask, doctorsTask, typesTask);

        var fees = await feesTask;
        _doctorNames = (await doctorsTask).Items.ToDictionary(d => d.Id, d => d.FullName);
        _examTypeNames = (await typesTask).Items.ToDictionary(t => t.Id, t => $"{t.Code} - {t.Name}");

        return fees;
    }

    private string ResolveDoctor(string doctorId)
        => _doctorNames.TryGetValue(doctorId, out var name) ? name : "-";

    private string ResolveExamType(string examTypeId)
        => _examTypeNames.TryGetValue(examTypeId, out var name) ? name : "-";

    private async Task OpenCreateDialogAsync()
    {
        var dialog = await DialogService.ShowAsync<ReferralFeeEditorDialog>(T.ReferralFee.NewReferralFee, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(ReferralFeeDto fee)
    {
        var parameters = new DialogParameters { ["Fee"] = fee };
        var dialog = await DialogService.ShowAsync<ReferralFeeEditorDialog>(T.ReferralFee.EditReferralFee, parameters, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task ToggleActiveAsync(ReferralFeeDto fee)
    {
        if (!await ConfirmDialogs.ConfirmStatusChangeAsync(DialogService, T, T.ReferralFee.ToggleStatus, ResolveDoctor(fee.ReferralDoctorId), !fee.IsActive))
            return;

        await SafeExecute.RunAsync(async () =>
            {
                if (fee.IsActive)
                    await PayrollService.DeactivateReferralFeeAsync(fee.Id);
                else
                    await PayrollService.ActivateReferralFeeAsync(fee.Id);

                Snackbar.Add(fee.IsActive ? T.ReferralFee.Deactivated : T.ReferralFee.Activated, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.ReferralFee.Unreachable);
    }

    private async Task DeleteFeeAsync(ReferralFeeDto fee)
    {
        var parameters = new DialogParameters
        {
            ["Title"] = T.ReferralFee.DeleteTitle,
            ["Message"] = T.ReferralFee.DeleteConfirm,
            ["Icon"] = Icons.Material.Filled.Delete,
            ["Color"] = MudBlazor.Color.Error,
            ["ConfirmText"] = T.Common.Delete,
            ["CancelText"] = T.Common.Cancel,
        };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, options);
        var result = await dialog.Result;

        if (result?.Canceled != false)
            return;

        await SafeExecute.RunAsync(async () =>
            {
                await PayrollService.DeleteReferralFeeAsync(fee.Id);
                Snackbar.Add(T.ReferralFee.Deleted, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.ReferralFee.Unreachable);
    }
}
