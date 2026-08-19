using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Components.Pages.Suppliers;

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

    private sealed class SupplierFormModel
    {
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(200, ErrorMessage = "Name must be 200 characters or fewer.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required.")]
        [MaxLength(30, ErrorMessage = "Phone must be 30 characters or fewer.")]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "Contact person must be 100 characters or fewer.")]
        public string? ContactPerson { get; set; }

        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string? Email { get; set; }

        [MaxLength(300, ErrorMessage = "Address must be 300 characters or fewer.")]
        public string? Address { get; set; }

        [MaxLength(50, ErrorMessage = "Tax number must be 50 characters or fewer.")]
        public string? TaxNumber { get; set; }

        [MaxLength(200, ErrorMessage = "Payment terms must be 200 characters or fewer.")]
        public string? PaymentTerms { get; set; }
    }
}