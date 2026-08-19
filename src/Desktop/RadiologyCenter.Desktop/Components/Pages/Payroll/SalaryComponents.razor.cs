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

public partial class SalaryComponents : ListPageBase<SalaryComponentDto>
{
    protected override string UnreachableMessage => T.SalaryComponent.Unreachable;

    protected override async Task<PagedResult<SalaryComponentDto>> LoadPageAsync(
        string? search,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken ct)
        => await PayrollService.GetSalaryComponentsPagedAsync(search, sortBy, sortDescending, page, pageSize, ct);

    private async Task OpenCreateDialogAsync()
    {
        var dialog = await DialogService.ShowAsync<SalaryComponentEditorDialog>(T.SalaryComponent.NewSalaryComponent, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(SalaryComponentDto component)
    {
        var parameters = new DialogParameters { ["Component"] = component };
        var dialog = await DialogService.ShowAsync<SalaryComponentEditorDialog>(T.FormatValue(T.SalaryComponent.EditTitle, component.Name), parameters, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task ToggleActiveAsync(SalaryComponentDto component)
    {
        await SafeExecute.RunAsync(async () =>
            {
                if (component.IsActive)
                    await PayrollService.DeactivateSalaryComponentAsync(component.Id);
                else
                    await PayrollService.ActivateSalaryComponentAsync(component.Id);

                Snackbar.Add(component.IsActive ? T.SalaryComponent.Deactivated : T.SalaryComponent.Activated, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.SalaryComponent.Unreachable);
    }

    private async Task DeleteComponentAsync(SalaryComponentDto component)
    {
        var confirmed = await DialogService.ShowMessageBoxAsync(
            T.SalaryComponent.DeleteTitle,
            T.FormatValue(T.SalaryComponent.DeleteConfirm, component.Name),
            T.Common.Delete,
            T.Common.Cancel);

        if (confirmed != true)
            return;

        await SafeExecute.RunAsync(async () =>
            {
                await PayrollService.DeleteSalaryComponentAsync(component.Id);
                Snackbar.Add(T.SalaryComponent.Deleted, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.SalaryComponent.Unreachable);
    }
}