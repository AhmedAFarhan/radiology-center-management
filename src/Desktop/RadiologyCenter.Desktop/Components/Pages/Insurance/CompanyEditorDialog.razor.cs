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

namespace RadiologyCenter.Desktop.Components.Pages.Insurance;

public partial class CompanyEditorDialog : ComponentBase
{
[Parameter] public InsuranceCompanyDto? Company { get; set; }

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private readonly CompanyFormModel _model = new();
    private EditContext _editContext = default!;
    private bool _busy;

    private bool IsEdit => Company is not null;

    protected override void OnInitialized()
    {
        _editContext = new EditContext(_model);

        if (Company is null)
            return;

        _model.Name = Company.Name;
        _model.TaxId = Company.TaxId;
        _model.Phone = Company.Phone;
        _model.Email = Company.Email;
        _model.Address = Company.Address;
    }

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        var input = new InsuranceCompanyInput
        {
            Name = _model.Name,
            TaxId = _model.TaxId,
            Phone = _model.Phone,
            Email = _model.Email,
            Address = _model.Address,
        };

        if (await SafeExecute.RunAsync(
                () => IsEdit
                    ? InsuranceService.UpdateCompanyAsync(Company!.Id, input)
                    : InsuranceService.CreateCompanyAsync(input),
                Snackbar,
                () => T.CompanyDialog.Unreachable,
                busy => _busy = busy))
        {
            Snackbar.Add(IsEdit ? T.CompanyDialog.Updated : T.CompanyDialog.Created, Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
    }

    private void CancelAsync()
        => MudDialog.Cancel();

    private sealed class CompanyFormModel
    {
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(200, ErrorMessage = "Name must be 200 characters or fewer.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "Tax number must be 50 characters or fewer.")]
        public string? TaxId { get; set; }

        [MaxLength(30, ErrorMessage = "Phone must be 30 characters or fewer.")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string? Email { get; set; }

        [MaxLength(300, ErrorMessage = "Address must be 300 characters or fewer.")]
        public string? Address { get; set; }
    }
}