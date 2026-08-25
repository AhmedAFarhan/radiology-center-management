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

public partial class Salaries : ListPageBase<SalaryDto>
{
    private IReadOnlyDictionary<string, string> _staffNames = new Dictionary<string, string>();

    protected override string UnreachableMessage => T.Salary.Unreachable;

    protected override async Task<PagedResult<SalaryDto>> LoadPageAsync(
        string? search,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        var salariesTask = PayrollService.GetSalariesPagedAsync(search, sortBy, sortDescending, page, pageSize, ct);
        var staffTask = ResourceService.GetStaffsPagedAsync(null, null, false, 1, 100, ct);

        await Task.WhenAll(salariesTask, staffTask);

        var salaries = await salariesTask;
        _staffNames = (await staffTask).Items.ToDictionary(s => s.Id, s => s.FullName);

        return salaries;
    }

    private string ResolveStaff(string staffId)
        => _staffNames.TryGetValue(staffId, out var name) ? name : "-";

    private async Task OpenCreateDialogAsync()
    {
        var dialog = await DialogService.ShowAsync<SalaryEditorDialog>(T.Salary.NewSalary, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(SalaryDto salary)
    {
        var parameters = new DialogParameters { ["Salary"] = salary };
        var dialog = await DialogService.ShowAsync<SalaryEditorDialog>(T.Salary.EditTitle, parameters, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task ToggleActiveAsync(SalaryDto salary)
    {
        if (!await ConfirmDialogs.ConfirmStatusChangeAsync(DialogService, T, T.Salary.ToggleStatus, ResolveStaff(salary.StaffId), !salary.IsActive))
            return;

        await SafeExecute.RunAsync(async () =>
            {
                if (salary.IsActive)
                    await PayrollService.DeactivateSalaryAsync(salary.Id);
                else
                    await PayrollService.ActivateSalaryAsync(salary.Id);

                Snackbar.Add(salary.IsActive ? T.Salary.Deactivated : T.Salary.Activated, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Salary.Unreachable);
    }

    private async Task DeleteSalaryAsync(SalaryDto salary)
    {
        var parameters = new DialogParameters
        {
            ["Title"] = T.Salary.DeleteTitle,
            ["Message"] = T.Salary.DeleteConfirm,
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
                await PayrollService.DeleteSalaryAsync(salary.Id);
                Snackbar.Add(T.Salary.Deleted, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Salary.Unreachable);
    }
}
