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

namespace RadiologyCenter.Desktop.Components.Pages.Examinations;

public partial class Examinations : ListPageBase<ExaminationTypeDto>
{
    protected override string BaseRoute => "/examinations";

    protected override string UnreachableMessage => T.Examinations.Unreachable;

    protected override async Task<PagedResult<ExaminationTypeDto>> LoadPageAsync(
        string? search,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken ct)
        => await ExaminationService.GetTypesPagedAsync(search, sortBy, sortDescending, page, pageSize, ct);

    protected override async Task OpenByDeepLinkAsync(string id)
    {
        ExaminationTypeDto? type = null;
        var ok = await SafeExecute.RunAsync(
            async () => type = await ExaminationService.GetTypeByIdAsync(id),
            Snackbar,
            () => T.Examinations.Unreachable);

        if (ok && type is not null)
        {
            var parameters = new DialogParameters { ["Type"] = type };
            var dialog = await DialogService.ShowAsync<ExaminationTypeEditorDialog>(T.Examinations.EditType, parameters, EditorDialogOptions);
            await ReloadIfSavedAsync(dialog);
        }

        NavigationManager.NavigateTo(BaseRoute, replace: true);
    }

    private async Task OpenCreateDialogAsync()
    {
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<ExaminationTypeEditorDialog>(T.Examinations.NewType, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(ExaminationTypeDto type)
    {
        var parameters = new DialogParameters { ["Type"] = type };
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<ExaminationTypeEditorDialog>(T.Examinations.EditType, parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task ToggleActiveAsync(ExaminationTypeDto type)
    {
        if (!await ConfirmDialogs.ConfirmStatusChangeAsync(DialogService, T, T.Examinations.ToggleStatus, type.Name, !type.IsActive))
            return;

        await SafeExecute.RunAsync(async () =>
            {
                if (type.IsActive)
                    await ExaminationService.DeactivateTypeAsync(type.Id);
                else
                    await ExaminationService.ActivateTypeAsync(type.Id);

                Snackbar.Add(type.IsActive ? T.Examinations.Deactivated : T.Examinations.Activated, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Examinations.Unreachable);
    }

    private async Task DeleteTypeAsync(ExaminationTypeDto type)
    {
        var parameters = new DialogParameters
        {
            ["Title"] = T.Examinations.DeleteTitle,
            ["Message"] = T.FormatValue(T.Examinations.DeleteConfirm, type.Name),
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
                await ExaminationService.DeleteTypeAsync(type.Id);
                Snackbar.Add(T.Examinations.Deleted, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Examinations.Unreachable);
    }
}