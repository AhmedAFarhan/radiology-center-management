using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;

using RadiologyCenter.Desktop.Features.Insurance.Models;

namespace RadiologyCenter.Desktop.Features.Insurance.Pages;

public partial class PolicyEditorDialog : EditorDialogBase
{
    private readonly PolicyFormModel _model = new();
    private EditContext _editContext = default!;
    private PatientDto? _selectedPatient;
    private InsuranceCompanyDto? _selectedCompany;

    protected override void OnInitialized()
    {
        _editContext = new EditContext(_model);
        _model.EffectiveFrom ??= DateTime.Today;
    }

    private async Task<IEnumerable<PatientDto>> SearchPatientsAsync(string? value, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Array.Empty<PatientDto>();

        var page = await SafeExecute.RunAsync(
            () => PatientService.GetPagedAsync(value, "LastName", false, 1, 20, ct),
            Snackbar,
            () => T.PolicyDialog.UnableSearchPatients);
        return page?.Items ?? Array.Empty<PatientDto>();
    }

    private async Task<IEnumerable<InsuranceCompanyDto>> SearchCompaniesAsync(string? value, CancellationToken ct)
    {
        var companies = await SafeExecute.RunAsync(
            () => InsuranceService.GetCompaniesAsync(ct),
            Snackbar,
            () => T.PolicyDialog.UnableSearchCompanies) ?? Array.Empty<InsuranceCompanyDto>();

        if (string.IsNullOrWhiteSpace(value))
            return companies;

        return companies.Where(c => c.Name.Contains(value, StringComparison.OrdinalIgnoreCase));
    }

    private async Task SubmitAsync()
    {
        _model.PatientId = _selectedPatient?.Id ?? string.Empty;
        _model.CompanyId = _selectedCompany?.Id ?? string.Empty;

        if (!_editContext.Validate())
            return;

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

        if (await TrySaveAsync(
                () => InsuranceService.CreatePolicyAsync(input),
                () => T.PolicyDialog.UnreachableRetry))
        {
            Snackbar.Add(T.PolicyDialog.Created, Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
    }

}
