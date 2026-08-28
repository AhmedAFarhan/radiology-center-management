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
using RadiologyCenter.Desktop.Features.Inventory.Models;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Features.Inventory.Components;

public partial class PurchaseOrderEditorDialog : EditorDialogBase
{
    private readonly PurchaseOrderFormModel _model = new();
    private EditContext _editContext = default!;
    private List<SupplierDto> _suppliers = new();
    private List<ItemDto> _items = new();

    protected override async Task OnInitializedAsync()
    {
        _editContext = new EditContext(_model);

        await SafeExecute.RunAsync(async () =>
            {
                var suppliersTask = InventoryService.GetSuppliersPagedAsync(null, null, false, 1, 200);
                var itemsTask = InventoryService.GetItemsPagedAsync(null, "Name", false, 1, 200);

                _suppliers = (await suppliersTask).Items.ToList();
                _items = (await itemsTask).Items.ToList();
            },
            Snackbar,
            () => T.PoDialog.LoadOptionsError);
    }

    private void AddLine() => _model.Items.Add(new PurchaseOrderLineModel());

    private void RemoveLine(PurchaseOrderLineModel line) => _model.Items.Remove(line);

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        var missingItems = _model.Items.Any(i => string.IsNullOrWhiteSpace(i.ItemId));
        var duplicateItems = _model.Items.GroupBy(i => i.ItemId).Any(g => g.Count() > 1);
        var invalidQuantities = _model.Items.Any(i => i.QuantityOrdered <= 0);

        if (missingItems || duplicateItems || invalidQuantities)
        {
            Snackbar.Add(T.PoDialog.LineValidationError, Severity.Warning);
            return;
        }

        await TrySaveAsync(async () =>
            {
                var input = new CreatePurchaseOrderInput
                {
                    SupplierId = _model.SupplierId,
                    ExpectedDeliveryAt = _model.ExpectedDeliveryAt,
                    Notes = _model.Notes,
                    Items = _model.Items.Select(i => new PurchaseOrderLineInput
                    {
                        ItemId = i.ItemId,
                        QuantityOrdered = i.QuantityOrdered,
                        UnitCost = i.UnitCost,
                    }).ToList(),
                };

                await InventoryService.CreatePurchaseOrderAsync(input);
                Snackbar.Add(T.PoDialog.PurchaseOrderCreated, Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            },
            () => T.PoDialog.UnreachableRetry);
    }

}