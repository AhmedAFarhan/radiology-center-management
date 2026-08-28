using System.ComponentModel.DataAnnotations;

using RadiologyCenter.Desktop.Features.Inventory.Models;

namespace RadiologyCenter.Desktop.Features.Inventory.Pages;

public partial class ItemEditorDialog : EditorDialogBase
{
    [Parameter] public ItemDto? Item { get; set; }

    private IReadOnlyList<EnumOptionDto> _categoryOptions = Array.Empty<EnumOptionDto>();
    private IReadOnlyList<EnumOptionDto> _unitOptions = Array.Empty<EnumOptionDto>();

    private readonly ItemFormModel _model = new();
    private EditContext _editContext = default!;

    private bool IsEdit => Item is not null;

    protected override async Task OnInitializedAsync()
    {
        _editContext = new EditContext(_model);

        await SafeExecute.RunAsync(async () =>
            {
                _categoryOptions = await EnumOptionsService.GetOptionsAsync("ItemCategory");
                _unitOptions = await EnumOptionsService.GetOptionsAsync("UnitType");
            },
            Snackbar,
            () => T.ItemDialog.Unreachable);

        if (Item is null)
            return;

        _model.Name = Item.Name;
        _model.Brand = Item.Brand;
        _model.Category = Item.CategoryKey;
        _model.Unit = Item.UnitKey;
        _model.ReorderLevel = Item.ReorderLevel;
        _model.ReorderQuantity = Item.ReorderQuantity;
        _model.LotTracked = Item.LotTracked ? "Yes" : "No";
        _model.StorageInstructions = Item.StorageInstructions;
    }

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        var input = new ItemInput
        {
            Name = _model.Name,
            Brand = _model.Brand,
            Category = _model.Category,
            Unit = _model.Unit,
            ReorderLevel = _model.ReorderLevel,
            ReorderQuantity = _model.ReorderQuantity,
            LotTracked = string.Equals(_model.LotTracked, "Yes", StringComparison.OrdinalIgnoreCase),
            StorageInstructions = _model.StorageInstructions,
        };

        if (await TrySaveAsync(
                () => IsEdit
                    ? InventoryService.UpdateItemAsync(Item!.Id, input)
                    : InventoryService.CreateItemAsync(input),
                () => T.ItemDialog.Unreachable))
        {
            Snackbar.Add(IsEdit ? T.ItemDialog.ItemUpdated : T.ItemDialog.ItemCreated, Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
    }

}
