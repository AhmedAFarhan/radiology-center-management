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

namespace RadiologyCenter.Desktop.Components.Pages.PurchaseOrders;

public partial class PurchaseOrders : ComponentBase, IDisposable
{
private MudTable<PurchaseOrderDto>? _table;
    private string? _search;
    private CancellationTokenSource? _searchCts;
    private string? _loadError;
    private bool _offline;

    private async Task<TableData<PurchaseOrderDto>> LoadServerData(TableState state, CancellationToken ct)
    {
        try
        {
            var page = await InventoryService.GetPurchaseOrdersPagedAsync(
                _search,
                state.SortLabel,
                state.SortDirection == SortDirection.Descending,
                state.Page + 1,
                state.PageSize,
                ct);

            _loadError = null;
            _offline = false;
            return new TableData<PurchaseOrderDto> { Items = page.Items, TotalItems = page.TotalCount };
        }
        catch (OperationCanceledException)
        {
            if (ct.IsCancellationRequested)
                return new TableData<PurchaseOrderDto> { Items = Array.Empty<PurchaseOrderDto>(), TotalItems = 0 };
            throw;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            _loadError = ex.Message;
            _offline = false;
            return new TableData<PurchaseOrderDto> { Items = Array.Empty<PurchaseOrderDto>(), TotalItems = 0 };
        }
        catch (Exception)
        {
            Snackbar.Add(T.PurchaseOrders.Unreachable, Severity.Error);
            _loadError = T.PurchaseOrders.Unreachable;
            _offline = true;
            return new TableData<PurchaseOrderDto> { Items = Array.Empty<PurchaseOrderDto>(), TotalItems = 0 };
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
        var dialog = await DialogService.ShowAsync<PurchaseOrderEditorDialog>(T.PoDialog.NewPurchaseOrder, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenReceiveDialogAsync(PurchaseOrderDto po)
    {
        var parameters = new DialogParameters { ["PurchaseOrderId"] = po.Id };
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<PurchaseOrderReceiveDialog>(T.FormatValue(T.PoDialog.ReceiveOrderTitle, po.OrderNumber), parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenViewDialogAsync(PurchaseOrderDto po)
    {
        var parameters = new DialogParameters { ["PurchaseOrderId"] = po.Id };
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        await DialogService.ShowAsync<PurchaseOrderViewDialog>(T.FormatValue(T.PoDialog.OrderTitle, po.OrderNumber), parameters, options);
    }

    private async Task ReloadIfSavedAsync(IDialogReference dialog)
    {
        var result = await dialog.Result;
        if (result is { Canceled: false })
            await ReloadAsync();
    }

    private async Task PlaceAsync(PurchaseOrderDto po)
    {
        var confirmed = await DialogService.ShowMessageBoxAsync(
            T.PurchaseOrders.PlaceOrderTitle,
            T.FormatValue(T.PurchaseOrders.PlaceOrderConfirm, po.OrderNumber),
            T.PurchaseOrders.PlaceOrderTitle,
            T.Common.Cancel);

        if (confirmed != true)
            return;

        await SafeExecute.RunAsync(async () =>
            {
                await InventoryService.PlacePurchaseOrderAsync(po.Id);
                Snackbar.Add(T.PurchaseOrders.OrderPlaced, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.PurchaseOrders.Unreachable);
    }

    private async Task CancelAsync(PurchaseOrderDto po)
    {
        var confirmed = await DialogService.ShowMessageBoxAsync(
            T.PurchaseOrders.CancelOrderTitle,
            T.FormatValue(T.PurchaseOrders.CancelOrderConfirm, po.OrderNumber),
            T.PurchaseOrders.CancelOrderTitle,
            T.PurchaseOrders.KeepOrder);

        if (confirmed != true)
            return;

        await SafeExecute.RunAsync(async () =>
            {
                await InventoryService.CancelPurchaseOrderAsync(po.Id);
                Snackbar.Add(T.PurchaseOrders.OrderCancelled, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.PurchaseOrders.Unreachable);
    }

    private static string FormatStatus(string status) => status switch
    {
        "PartiallyReceived" => "Partially Received",
        _ => status,
    };

    public void Dispose() => _searchCts?.Cancel();
}