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

namespace RadiologyCenter.Desktop.Components.Pages.Resources;

public partial class Equipment : ComponentBase, IDisposable
{
private MudTable<EquipmentDto>? _table;
    private string? _search;
    private CancellationTokenSource? _searchCts;
    private string? _loadError;
    private bool _offline;

    private async Task<TableData<EquipmentDto>> LoadServerData(TableState state, CancellationToken ct)
    {
        try
        {
            var page = await ResourceService.GetEquipmentPagedAsync(
                _search,
                state.SortLabel,
                state.SortDirection == SortDirection.Descending,
                state.Page + 1,
                state.PageSize,
                ct);

            _loadError = null;
            _offline = false;
            return new TableData<EquipmentDto> { Items = page.Items, TotalItems = page.TotalCount };
        }
        catch (OperationCanceledException)
        {
            if (ct.IsCancellationRequested)
                return new TableData<EquipmentDto> { Items = Array.Empty<EquipmentDto>(), TotalItems = 0 };
            throw;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            _loadError = ex.Message;
            _offline = false;
            return new TableData<EquipmentDto> { Items = Array.Empty<EquipmentDto>(), TotalItems = 0 };
        }
        catch (Exception)
        {
            Snackbar.Add(T.Equipment.Unreachable, Severity.Error);
            _loadError = T.Equipment.Unreachable;
            _offline = true;
            return new TableData<EquipmentDto> { Items = Array.Empty<EquipmentDto>(), TotalItems = 0 };
        }
    }

    private async Task OnSearchChanged(string? value)
    {
        _search = value;

        _searchCts?.Cancel();
        var cts = _searchCts = new CancellationTokenSource();
        try
        {
            await Task.Delay(400, cts.Token);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        if (_table is not null)
            await _table.ReloadServerData();
    }

    private Task ReloadAsync()
        => _table is null ? Task.CompletedTask : _table.ReloadServerData();

    private async Task OpenCreateDialogAsync()
    {
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<EquipmentEditorDialog>(T.Equipment.NewEquipment, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(EquipmentDto equipment)
    {
        var parameters = new DialogParameters { ["Equipment"] = equipment };
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<EquipmentEditorDialog>(T.FormatValue(T.Equipment.Edit, equipment.Name), parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenStatusDialogAsync(EquipmentDto equipment)
    {
        var parameters = new DialogParameters { ["Equipment"] = equipment };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<EquipmentStatusDialog>(T.FormatValue(T.Equipment.SetStatusTitle, equipment.Name), parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task ReloadIfSavedAsync(IDialogReference dialog)
    {
        var result = await dialog.Result;
        if (result is { Canceled: false })
            await ReloadAsync();
    }

    private async Task ToggleActiveAsync(EquipmentDto equipment)
    {
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
        var confirmed = await DialogService.ShowMessageBoxAsync(
            T.Equipment.DeleteTitle,
            T.FormatValue(T.Equipment.DeleteConfirm, equipment.Name),
            T.Common.Delete,
            T.Common.Cancel);

        if (confirmed != true)
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

    private static string FormatModality(string modality) => modality switch
    {
        "XRay" => "X-Ray",
        "Ultrasound" => "Ultrasound",
        "Mammography" => "Mammography",
        "Fluoroscopy" => "Fluoroscopy",
        _ => modality,
    };

    private static string FormatStatus(string status) => status switch
    {
        "UnderMaintenance" => "Under Maintenance",
        "OutOfService" => "Out of Service",
        _ => status,
    };

    public void Dispose() => _searchCts?.Cancel();
}