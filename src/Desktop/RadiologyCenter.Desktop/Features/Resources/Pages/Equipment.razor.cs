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

public partial class Equipment : ListPageBase<EquipmentDto>
{
    protected override string UnreachableMessage => T.Equipment.Unreachable;

    protected override async Task<PagedResult<EquipmentDto>> LoadPageAsync(
        string? search,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken ct)
        => await ResourceService.GetEquipmentPagedAsync(search, sortBy, sortDescending, page, pageSize, ct);

    private async Task OpenCreateDialogAsync()
    {
        var dialog = await DialogService.ShowAsync<EquipmentEditorDialog>(T.Equipment.NewEquipment, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(EquipmentDto equipment)
    {
        var parameters = new DialogParameters { ["Equipment"] = equipment };
        var dialog = await DialogService.ShowAsync<EquipmentEditorDialog>(T.FormatValue(T.Equipment.Edit, equipment.Name), parameters, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenStatusDialogAsync(EquipmentDto equipment)
    {
        var parameters = new DialogParameters { ["Equipment"] = equipment };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<EquipmentStatusDialog>(T.FormatValue(T.Equipment.SetStatusTitle, equipment.Name), parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task ToggleActiveAsync(EquipmentDto equipment)
    {
        if (!await ConfirmDialogs.ConfirmStatusChangeAsync(DialogService, T, T.Equipment.ToggleStatus, equipment.Name, !equipment.IsActive))
            return;

        await SafeExecute.RunAsync(async () =>
            {
                if (equipment.IsActive)
                    await ResourceService.DeactivateEquipmentAsync(equipment.Id);
                else
                    await ResourceService.ActivateEquipmentAsync(equipment.Id);

                Snackbar.Add(equipment.IsActive ? T.Equipment.Deactivated : T.Equipment.Activated, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Equipment.Unreachable);
    }

    private async Task DeleteEquipmentAsync(EquipmentDto equipment)
    {
        var parameters = new DialogParameters
        {
            ["Title"] = T.Equipment.DeleteTitle,
            ["Message"] = T.FormatValue(T.Equipment.DeleteConfirm, equipment.Name),
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
                await ResourceService.DeleteEquipmentAsync(equipment.Id);
                Snackbar.Add(T.Equipment.Deleted, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Equipment.Unreachable);
    }

    private string FormatStatus(string status) => status switch
    {
        "UnderMaintenance" => T.Equipment.StatusUnderMaintenance,
        "OutOfService" => T.Equipment.StatusOutOfService,
        _ => status,
    };
}
