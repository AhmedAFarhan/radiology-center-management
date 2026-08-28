using System.ComponentModel.DataAnnotations;

using RadiologyCenter.Desktop.Features.Inventory.Models;

namespace RadiologyCenter.Desktop.Features.Inventory.Components;

public partial class SupplierEditorDialog : EditorDialogBase
{
    [Parameter] public SupplierDto? Supplier { get; set; }

    private readonly SupplierFormModel _model = new();
    private EditContext _editContext = default!;

    private bool IsEdit => Supplier is not null;

    protected override void OnInitialized()
    {
        _editContext = new EditContext(_model);

        if (Supplier is null)
            return;

        _model.Name = Supplier.Name;
        _model.Phone = Supplier.Phone;
        _model.ContactPerson = Supplier.ContactPerson;
        _model.Email = Supplier.Email;
        _model.Address = Supplier.Address;
        _model.TaxNumber = Supplier.TaxNumber;
        _model.PaymentTerms = Supplier.PaymentTerms;
    }

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        var input = new SupplierInput
        {
            Name = _model.Name,
            Phone = _model.Phone,
            ContactPerson = _model.ContactPerson,
            Email = _model.Email,
            Address = _model.Address,
            TaxNumber = _model.TaxNumber,
            PaymentTerms = _model.PaymentTerms,
        };

        if (await TrySaveAsync(
                () => IsEdit
                    ? InventoryService.UpdateSupplierAsync(Supplier!.Id, input)
                    : InventoryService.CreateSupplierAsync(input),
                () => T.SupplierDialog.Unreachable))
        {
            Snackbar.Add(IsEdit ? T.SupplierDialog.SupplierUpdated : T.SupplierDialog.SupplierCreated, Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
    }

}
