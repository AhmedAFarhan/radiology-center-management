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

namespace RadiologyCenter.Desktop.Components.Pages.Inventory;

public partial class Inventory : ComponentBase, IDisposable
{
private MudTable<ItemDto>? _table;
    private string? _search;
    private CancellationTokenSource? _searchCts;
    private string? _loadError;
    private bool _offline;
    private string? _openId;

    [SupplyParameterFromQuery(Name = "q")]
    public string? SearchQuery { get; set; }

    [SupplyParameterFromQuery(Name = "open")]
    public string? OpenId { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (!string.IsNullOrWhiteSpace(OpenId) && Guid.TryParse(OpenId, out _))
            _openId = OpenId;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_openId is not null)
        {
            var id = _openId;
            _openId = null;
            await OpenByDeepLinkAsync(id);
        }
    }

    private async Task OpenByDeepLinkAsync(string id)
    {
        ItemDto? item = null;
        var ok = await SafeExecute.RunAsync(
            async () => item = await InventoryService.GetItemByIdAsync(id),
            Snackbar,
            () => T.Inventory.Unreachable);

        if (ok && item is not null)
        {
            var parameters = new DialogParameters { ["Item"] = item };
            var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
            var dialog = await DialogService.ShowAsync<ItemEditorDialog>(T.ItemDialog.EditItem, parameters, options);
            await ReloadIfSavedAsync(dialog);
        }

        NavigationManager.NavigateTo("/inventory/items", replace: true);
    }

    private async Task<TableData<ItemDto>> LoadServerData(TableState state, CancellationToken ct)
    {
        try
        {
            var page = await InventoryService.GetItemsPagedAsync(
                _search,
                state.SortLabel,
                state.SortDirection == SortDirection.Descending,
                state.Page + 1,
                state.PageSize,
                ct);

            _loadError = null;
            _offline = false;
            return new TableData<ItemDto> { Items = page.Items, TotalItems = page.TotalCount };
        }
        catch (OperationCanceledException)
        {
            if (ct.IsCancellationRequested)
                return new TableData<ItemDto> { Items = Array.Empty<ItemDto>(), TotalItems = 0 };
            throw;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            _loadError = ex.Message;
            _offline = false;
            return new TableData<ItemDto> { Items = Array.Empty<ItemDto>(), TotalItems = 0 };
        }
        catch (Exception)
        {
            Snackbar.Add(T.Inventory.Unreachable, Severity.Error);
            _loadError = T.Inventory.Unreachable;
            _offline = true;
            return new TableData<ItemDto> { Items = Array.Empty<ItemDto>(), TotalItems = 0 };
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
        var dialog = await DialogService.ShowAsync<ItemEditorDialog>(T.ItemDialog.NewItem, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(ItemDto item)
    {
        var parameters = new DialogParameters { ["Item"] = item };
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<ItemEditorDialog>(T.ItemDialog.EditItem, parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenStockDialogAsync(ItemDto item)
    {
        var parameters = new DialogParameters
        {
            ["ItemId"] = item.Id,
        };
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        await DialogService.ShowAsync<ItemStockDialog>(item.Name, parameters, options);
    }

    private async Task ReloadIfSavedAsync(IDialogReference dialog)
    {
        var result = await dialog.Result;
        if (result is { Canceled: false })
            await ReloadAsync();
    }

    private async Task ToggleActiveAsync(ItemDto item)
    {
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
        var confirmed = await DialogService.ShowMessageBoxAsync(
            T.Inventory.DeleteItemTitle,
            T.FormatValue(T.Inventory.DeleteConfirm, item.Name),
            T.Common.Delete,
            T.Common.Cancel);

        if (confirmed != true)
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

    private static string FormatCategory(string category) => category switch
    {
        "ContrastMedia" => "Contrast Media",
        "MedicalSupply" => "Medical Supply",
        _ => category,
    };

    public void Dispose() => _searchCts?.Cancel();
}