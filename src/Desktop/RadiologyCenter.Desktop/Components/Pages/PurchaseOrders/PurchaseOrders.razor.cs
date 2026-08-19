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

public partial class PurchaseOrders : ListPageBase<PurchaseOrderDto>
{
    protected override string UnreachableMessage => T.PurchaseOrders.Unreachable;

    protected override async Task<PagedResult<PurchaseOrderDto>> LoadPageAsync(
        string? search,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken ct)
        => await InventoryService.GetPurchaseOrdersPagedAsync(search, sortBy, sortDescending, page, pageSize, ct);

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
}