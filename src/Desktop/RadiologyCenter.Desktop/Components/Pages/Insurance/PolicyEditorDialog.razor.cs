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

public partial class PolicyEditorDialog : ComponentBase
{
[CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private readonly PolicyFormModel _model = new();
    private EditContext _editContext = default!;
    private PatientDto? _selectedPatient;
    private InsuranceCompanyDto? _selectedCompany;
    private bool _busy;

    protected override void OnInitialized()
    {
        _editContext = new EditContext(_model);
        _model.EffectiveFrom ??= DateTime.Today;
    }

    private async Task<IEnumerable<PatientDto>> SearchPatientsAsync(string? value, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Array.Empty<PatientDto>();

        try
        {
            var page = await PatientService.GetPagedAsync(value, "LastName", false, 1, 20, ct);
            return page.Items;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            return Array.Empty<PatientDto>();
        }
        catch (Exception)
        {
            Snackbar.Add(T.PolicyDialog.UnableSearchPatients, Severity.Error);
            return Array.Empty<PatientDto>();
        }
    }

    private async Task<IEnumerable<InsuranceCompanyDto>> SearchCompaniesAsync(string? value, CancellationToken ct)
    {
        try
        {
            var companies = await InsuranceService.GetCompaniesAsync(ct);
            if (string.IsNullOrWhiteSpace(value))
                return companies;

            return companies.Where(c => c.Name.Contains(value, StringComparison.OrdinalIgnoreCase));
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            return Array.Empty<InsuranceCompanyDto>();
        }
        catch (Exception)
        {
            Snackbar.Add(T.PolicyDialog.UnableSearchCompanies, Severity.Error);
            return Array.Empty<InsuranceCompanyDto>();
        }
    }

    private async Task SubmitAsync()
    {
        _model.PatientId = _selectedPatient?.Id ?? string.Empty;
        _model.CompanyId = _selectedCompany?.Id ?? string.Empty;

        if (!_editContext.Validate())
            return;

        await SafeExecute.RunAsync(async () =>
            {
                var input = new InsurancePolicyInput
                {
                    CompanyId = _model.CompanyId,
                    PatientId = _model.PatientId,
                    PolicyNumber = _model.PolicyNumber,
                    CoveragePercent = _model.CoveragePercent,
                    EffectiveFrom = _model.EffectiveFrom ?? DateTime.Today,
                    EffectiveTo = _model.EffectiveTo,
                    IsGovernment = _model.IsGovernment,
                };

                await InsuranceService.CreatePolicyAsync(input);
                Snackbar.Add(T.PolicyDialog.Created, Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            },
            Snackbar,
            () => T.PolicyDialog.UnreachableRetry,
            busy => _busy = busy);
    }

    private void CancelAsync()
        => MudDialog.Cancel();

    private sealed class PolicyFormModel : IValidatableObject
    {
        public string PatientId { get; set; } = string.Empty;
        public string CompanyId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Policy number is required.")]
        [MaxLength(100, ErrorMessage = "Policy number must be 100 characters or fewer.")]
        public string PolicyNumber { get; set; } = string.Empty;

        [Range(0, 100, ErrorMessage = "Coverage must be between 0 and 100.")]
        public decimal CoveragePercent { get; set; } = 100;

        public DateTime? EffectiveFrom { get; set; } = DateTime.Today;
        public DateTime? EffectiveTo { get; set; }
        public bool IsGovernment { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(PatientId))
                yield return new ValidationResult("Patient is required.", new[] { nameof(PatientId) });

            if (string.IsNullOrWhiteSpace(CompanyId))
                yield return new ValidationResult("Insurance company is required.", new[] { nameof(CompanyId) });

            if (EffectiveFrom is null)
                yield return new ValidationResult("Effective From is required.", new[] { nameof(EffectiveFrom) });

            if (EffectiveTo is not null && EffectiveFrom is not null && EffectiveTo.Value.Date < EffectiveFrom.Value.Date)
                yield return new ValidationResult(
                    "Effective To cannot be before Effective From.",
                    new[] { nameof(EffectiveTo) });
        }
    }
}