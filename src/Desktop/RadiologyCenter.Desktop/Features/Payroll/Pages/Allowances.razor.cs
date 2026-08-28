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

namespace RadiologyCenter.Desktop.Features.Payroll.Pages;

public partial class Allowances : ListPageBase<AllowanceAssignmentDto>
{
    private IReadOnlyDictionary<string, string> _staffNames = new Dictionary<string, string>();

    protected override string UnreachableMessage => T.Allowance.Unreachable;

    protected override async Task<PagedResult<AllowanceAssignmentDto>> LoadPageAsync(
        string? search,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        var allowancesTask = PayrollService.GetAllowancesPagedAsync(search, sortBy, sortDescending, page, pageSize, ct);
        var staffTask = ResourceService.GetStaffsPagedAsync(null, null, false, 1, 100, ct);

        await Task.WhenAll(allowancesTask, staffTask);

        var allowances = await allowancesTask;
        _staffNames = (await staffTask).Items.ToDictionary(s => s.Id, s => s.FullName);

        return allowances;
    }

    private string ResolveStaff(string staffId)
        => _staffNames.TryGetValue(staffId, out var name) ? name : "-";

    private async Task OpenCreateDialogAsync()
    {
        var dialog = await DialogService.ShowAsync<AllowanceEditorDialog>(T.Allowance.NewAllowance, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(AllowanceAssignmentDto allowance)
    {
        var parameters = new DialogParameters { ["Allowance"] = allowance };
        var dialog = await DialogService.ShowAsync<AllowanceEditorDialog>(T.FormatValue(T.Allowance.EditTitle, allowance.Name), parameters, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task ToggleActiveAsync(AllowanceAssignmentDto allowance)
    {
        if (!await ConfirmDialogs.ConfirmStatusChangeAsync(DialogService, T, T.Allowance.ToggleStatus, allowance.Name, !allowance.IsActive))
            return;

        await SafeExecute.RunAsync(async () =>
            {
                if (allowance.IsActive)
                    await PayrollService.DeactivateAllowanceAsync(allowance.Id);
                else
                    await PayrollService.ActivateAllowanceAsync(allowance.Id);

                Snackbar.Add(allowance.IsActive ? T.Allowance.Deactivated : T.Allowance.Activated, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Allowance.Unreachable);
    }

    private async Task DeleteAllowanceAsync(AllowanceAssignmentDto allowance)
    {
        var parameters = new DialogParameters
        {
            ["Title"] = T.Allowance.DeleteTitle,
            ["Message"] = T.FormatValue(T.Allowance.DeleteConfirm, allowance.Name),
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
                await PayrollService.DeleteAllowanceAsync(allowance.Id);
                Snackbar.Add(T.Allowance.Deleted, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Allowance.Unreachable);
    }
}
