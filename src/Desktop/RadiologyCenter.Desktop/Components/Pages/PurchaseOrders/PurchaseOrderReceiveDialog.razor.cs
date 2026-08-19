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

public partial class PurchaseOrderReceiveDialog : ComponentBase
{
[Parameter] public string PurchaseOrderId { get; set; } = string.Empty;

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private PurchaseOrderDto? _po;
    private List<ReceiveLineModel> _receiveLines = new();
    private string? _error;
    private bool _loading;
    private bool _busy;

    protected override async Task OnInitializedAsync()
        => await LoadAsync();

    private async Task LoadAsync()
    {
        _loading = true;
        _error = null;
        try
        {
            _po = await InventoryService.GetPurchaseOrderByIdAsync(PurchaseOrderId);
            _receiveLines = _po.Items.Select(i => new ReceiveLineModel
            {
                Id = i.Id,
                ItemId = i.ItemId,
                Quantity = i.QuantityOrdered - i.QuantityReceived,
            }).ToList();
        }
        catch (ApiException ex)
        {
            _error = ex.Message;
        }
        catch (Exception)
        {
            _error = T.PoDialog.Unreachable;
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task SubmitAsync()
    {
        if (_po is null)
            return;

        var lines = _receiveLines
            .Where(l => l.Quantity is > 0)
            .ToList();

        if (lines.Count == 0)
        {
            Snackbar.Add(T.PoDialog.QtyWarning, Severity.Warning);
            return;
        }

        if (lines.Any(l => string.IsNullOrWhiteSpace(l.LotNumber)))
        {
            Snackbar.Add(T.PoDialog.LotRequired, Severity.Warning);
            return;
        }

        if (lines.Any(l => l.ExpiryDate is not null && l.ExpiryDate.Value.Date < DateTime.Today))
        {
            Snackbar.Add(T.PoDialog.ExpiryInPast, Severity.Warning);
            return;
        }

        await SafeExecute.RunAsync(async () =>
            {
                var input = new ReceivePurchaseOrderInput
                {
                    Lines = lines.Select(l => new ReceivePurchaseOrderLineInput
                    {
                        ItemId = l.ItemId,
                        Quantity = l.Quantity!.Value,
                        LotNumber = l.LotNumber.Trim(),
                        ExpiryDate = l.ExpiryDate,
                    }).ToList(),
                };

                await InventoryService.ReceivePurchaseOrderAsync(_po.Id, input);
                Snackbar.Add(T.PoDialog.ItemsReceived, Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            },
            Snackbar,
            () => T.PoDialog.UnreachableRetry,
            busy => _busy = busy);
    }

    private void CancelAsync()
        => MudDialog.Cancel();

    private sealed class ReceiveLineModel
    {
        public string Id { get; set; } = string.Empty;
        public string ItemId { get; set; } = string.Empty;
        public int? Quantity { get; set; }
        public string LotNumber { get; set; } = string.Empty;
        public DateTime? ExpiryDate { get; set; }
    }
}