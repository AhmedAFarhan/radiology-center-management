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

namespace RadiologyCenter.Desktop.Components.Pages.Suppliers;

public partial class SupplierEditorDialog : ComponentBase
{
[Parameter] public SupplierDto? Supplier { get; set; }

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private readonly SupplierFormModel _model = new();
    private EditContext _editContext = default!;
    private bool _busy;

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

        if (await SafeExecute.RunAsync(
                () => IsEdit
                    ? InventoryService.UpdateSupplierAsync(Supplier!.Id, input)
                    : InventoryService.CreateSupplierAsync(input),
                Snackbar,
                () => T.SupplierDialog.Unreachable,
                busy => _busy = busy))
        {
            Snackbar.Add(IsEdit ? T.SupplierDialog.SupplierUpdated : T.SupplierDialog.SupplierCreated, Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
    }

    private void CancelAsync()
        => MudDialog.Cancel();

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