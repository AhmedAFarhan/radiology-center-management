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

namespace RadiologyCenter.Desktop.Features.Resources.Pages;

public partial class Staff : ListPageBase<StaffDto>
{
    protected override string BaseRoute => "/resources/staff";

    protected override string UnreachableMessage => T.Staff.Unreachable;

    protected override async Task<PagedResult<StaffDto>> LoadPageAsync(
        string? search,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken ct)
        => await ResourceService.GetStaffsPagedAsync(search, sortBy, sortDescending, page, pageSize, ct);

    protected override async Task OpenByDeepLinkAsync(string id)
    {
        StaffDto? staff = null;
        var ok = await SafeExecute.RunAsync(
            async () => { staff = await ResourceService.GetStaffByIdAsync(id); },
            Snackbar,
            () => T.Staff.Unreachable);

        if (ok && staff is not null)
        {
            var parameters = new DialogParameters { ["Staff"] = staff };
            var dialog = await DialogService.ShowAsync<StaffEditorDialog>(T.FormatValue(T.Staff.Edit, staff.FullName), parameters, EditorDialogOptions);
            await ReloadIfSavedAsync(dialog);
        }

        NavigationManager.NavigateTo(BaseRoute, replace: true);
    }

    private async Task OpenCreateDialogAsync()
    {
        var dialog = await DialogService.ShowAsync<StaffEditorDialog>(T.Staff.NewStaff, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(StaffDto staff)
    {
        var parameters = new DialogParameters { ["Staff"] = staff };
        var dialog = await DialogService.ShowAsync<StaffEditorDialog>(T.FormatValue(T.Staff.Edit, staff.FullName), parameters, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task ToggleActiveAsync(StaffDto staff)
    {
        if (!await ConfirmDialogs.ConfirmStatusChangeAsync(DialogService, T, T.Staff.ToggleStatus, staff.FullName, !staff.IsActive))
            return;

        await SafeExecute.RunAsync(async () =>
            {
                if (staff.IsActive)
                    await ResourceService.DeactivateStaffAsync(staff.Id);
                else
                    await ResourceService.ActivateStaffAsync(staff.Id);

                Snackbar.Add(staff.IsActive ? T.Staff.Deactivated : T.Staff.Activated, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Staff.Unreachable);
    }

    private async Task DeleteStaffAsync(StaffDto staff)
    {
        var parameters = new DialogParameters
        {
            ["Title"] = T.Staff.DeleteTitle,
            ["Message"] = T.FormatValue(T.Staff.DeleteConfirm, staff.FullName),
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
                await ResourceService.DeleteStaffAsync(staff.Id);
                Snackbar.Add(T.Staff.Deleted, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Staff.Unreachable);
    }
}
