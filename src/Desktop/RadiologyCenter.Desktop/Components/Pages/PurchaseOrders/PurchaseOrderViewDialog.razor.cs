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

public partial class PurchaseOrderViewDialog : ComponentBase
{
[Parameter] public string PurchaseOrderId { get; set; } = string.Empty;

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private PurchaseOrderDto? _po;
    private string? _error;
    private bool _loading;

    protected override async Task OnInitializedAsync()
        => await LoadAsync();

    private async Task LoadAsync()
    {
        _loading = true;
        _error = null;
        try
        {
            _po = await InventoryService.GetPurchaseOrderByIdAsync(PurchaseOrderId);
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

    private decimal Total()
        => _po is null ? 0 : _po.Items.Sum(i => i.QuantityOrdered * i.UnitCost);

    private void CloseAsync()
        => MudDialog.Close();
}