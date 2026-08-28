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

namespace RadiologyCenter.Desktop.Features.Inventory.Pages;

public partial class Inventory : ListPageBase<ItemDto>
{
    protected override string BaseRoute => "/inventory/items";

    protected override string UnreachableMessage => T.Inventory.Unreachable;

    protected override async Task<PagedResult<ItemDto>> LoadPageAsync(
        string? search,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken ct)
        => await InventoryService.GetItemsPagedAsync(search, sortBy, sortDescending, page, pageSize, ct);

    protected override async Task OpenByDeepLinkAsync(string id)
    {
        ItemDto? item = null;
        var ok = await SafeExecute.RunAsync(
            async () => { item = await InventoryService.GetItemByIdAsync(id); },
            Snackbar,
            () => T.Inventory.Unreachable);

        if (ok && item is not null)
        {
            var parameters = new DialogParameters { ["Item"] = item };
            var dialog = await DialogService.ShowAsync<ItemEditorDialog>(T.ItemDialog.EditItem, parameters, EditorDialogOptions);
            await ReloadIfSavedAsync(dialog);
        }

        NavigationManager.NavigateTo(BaseRoute, replace: true);
    }

    private async Task OpenCreateDialogAsync()
    {
        var dialog = await DialogService.ShowAsync<ItemEditorDialog>(T.ItemDialog.NewItem, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task ExportItemsToExcelAsync()
    {
        await SafeExecute.RunAsync(
            async () =>
            {
                var content = await InventoryService.ExportItemsAsync(Search);
                var path = await FileSaveHelper.SaveAsync(content, "inventory-items.xlsx");
                Snackbar.Add(T.FormatValue(T.Common.SavedTo, path), Severity.Success);
            },
            Snackbar,
            () => T.Inventory.Unreachable);
    }

    private async Task OpenItemsImportDialogAsync()
    {
        var parameters = new DialogParameters
        {
            ["DownloadTemplate"] = new Func<Task<byte[]>>(() => InventoryService.DownloadItemsImportTemplateAsync()),
            ["ImportFile"] = new Func<string, Stream, Task<ExcelImportResultDto>>((fileName, stream) => InventoryService.ImportItemsAsync(fileName, stream)),
            ["Imported"] = EventCallback.Factory.Create(this, ReloadAsync),
        };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader = true };
        await DialogService.ShowAsync<ExcelImportDialog>(string.Empty, parameters, options);
    }

    private async Task OpenEditDialogAsync(ItemDto item)
    {
        var parameters = new DialogParameters { ["Item"] = item };
        var dialog = await DialogService.ShowAsync<ItemEditorDialog>(T.ItemDialog.EditItem, parameters, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenStockDialogAsync(ItemDto item)
    {
        var parameters = new DialogParameters
        {
            ["ItemId"] = item.Id,
        };
        await DialogService.ShowAsync<ItemStockDialog>(item.Name, parameters, EditorDialogOptions);
    }

    private async Task ToggleActiveAsync(ItemDto item)
    {
        if (!await ConfirmDialogs.ConfirmStatusChangeAsync(DialogService, T, T.Inventory.ToggleStatus, item.Name, !item.IsActive))
            return;

        await SafeExecute.RunAsync(async () =>
            {
                if (item.IsActive)
                    await InventoryService.DeactivateItemAsync(item.Id);
                else
                    await InventoryService.ActivateItemAsync(item.Id);

                Snackbar.Add(item.IsActive ? T.Inventory.ItemDeactivated : T.Inventory.ItemActivated, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Inventory.Unreachable);
    }

    private async Task DeleteItemAsync(ItemDto item)
    {
        var parameters = new DialogParameters
        {
            ["Title"] = T.Inventory.DeleteItemTitle,
            ["Message"] = T.FormatValue(T.Inventory.DeleteConfirm, item.Name),
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
                await InventoryService.DeleteItemAsync(item.Id);
                Snackbar.Add(T.Inventory.ItemDeleted, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Inventory.Unreachable);
    }
}
