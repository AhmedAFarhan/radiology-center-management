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

public partial class ExaminationFees : ListPageBase<ExaminationFeeDto>
{
    private IReadOnlyDictionary<string, string> _examTypeNames = new Dictionary<string, string>();

    protected override string UnreachableMessage => T.ExamFee.Unreachable;

    protected override async Task<PagedResult<ExaminationFeeDto>> LoadPageAsync(
        string? search,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        var feesTask = PayrollService.GetExaminationFeesPagedAsync(search, sortBy, sortDescending, page, pageSize, ct);
        var typesTask = ExaminationService.GetTypesPagedAsync(null, null, false, 1, 100, ct);

        await Task.WhenAll(feesTask, typesTask);

        var fees = await feesTask;
        _examTypeNames = (await typesTask).Items.ToDictionary(t => t.Id, t => $"{t.Code} - {t.Name}");

        return fees;
    }

    private string ResolveExamType(string examTypeId)
        => _examTypeNames.TryGetValue(examTypeId, out var name) ? name : "-";

    private async Task OpenCreateDialogAsync()
    {
        var dialog = await DialogService.ShowAsync<ExaminationFeeEditorDialog>(T.ExamFee.NewExaminationFee, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(ExaminationFeeDto fee)
    {
        var parameters = new DialogParameters { ["Fee"] = fee };
        var dialog = await DialogService.ShowAsync<ExaminationFeeEditorDialog>(T.ExamFee.EditExaminationFee, parameters, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task ToggleActiveAsync(ExaminationFeeDto fee)
    {
        if (!await ConfirmDialogs.ConfirmStatusChangeAsync(DialogService, T, T.ExamFee.ToggleStatus, ResolveExamType(fee.ExaminationTypeId), !fee.IsActive))
            return;

        await SafeExecute.RunAsync(async () =>
            {
                if (fee.IsActive)
                    await PayrollService.DeactivateExaminationFeeAsync(fee.Id);
                else
                    await PayrollService.ActivateExaminationFeeAsync(fee.Id);

                Snackbar.Add(fee.IsActive ? T.ExamFee.Deactivated : T.ExamFee.Activated, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.ExamFee.Unreachable);
    }

    private async Task DeleteFeeAsync(ExaminationFeeDto fee)
    {
        var parameters = new DialogParameters
        {
            ["Title"] = T.ExamFee.DeleteTitle,
            ["Message"] = T.ExamFee.DeleteConfirm,
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
                await PayrollService.DeleteExaminationFeeAsync(fee.Id);
                Snackbar.Add(T.ExamFee.Deleted, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.ExamFee.Unreachable);
    }
}