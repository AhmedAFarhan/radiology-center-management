using System.ComponentModel.DataAnnotations;

using RadiologyCenter.Desktop.Features.Inventory.Models;

namespace RadiologyCenter.Desktop.Features.Inventory.Components;

public partial class ItemStockDialog : ComponentBase
{
[Parameter] public string ItemId { get; set; } = string.Empty;

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private ItemStockDto? _stock;
    private string? _loadError;
    private readonly IssueStockModel _model = new();
    private EditContext _editContext = default!;
    private bool _busy;

    protected override async Task OnInitializedAsync()
    {
        _editContext = new EditContext(_model);
        await LoadStockAsync();
    }

    private async Task LoadStockAsync()
    {
        _loadError = null;
        try
        {
            _stock = await InventoryService.GetStockAsync(ItemId);
        }
        catch (ApiException ex)
        {
            _loadError = ex.Message;
        }
        catch (Exception)
        {
            _loadError = T.Inventory.Unreachable;
        }
    }

    private async Task IssueAsync()
    {
        if (!_editContext.Validate())
            return;

        if (_model.Quantity is not { } qty)
            return;

        await SafeExecute.RunAsync(async () =>
            {
                await InventoryService.IssueStockAsync(ItemId, new IssueStockInput
                {
                    Quantity = qty,
                    Reference = _model.Reference,
                    Notes = _model.Notes,
                });

                Snackbar.Add(T.FormatValue(T.ItemDialog.IssuedUnits, qty), Severity.Success);
                _model.Quantity = null;
                _model.Reference = null;
                _model.Notes = null;
                await LoadStockAsync();
            },
            Snackbar,
            () => T.Inventory.Unreachable,
            busy => _busy = busy);
    }

    private void CancelAsync()
        => MudDialog.Cancel();

}
