using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Features.Insurance.Pages;

public partial class PreAuthDialog : EditorDialogBase
{
    private readonly PreAuthFormModel _model = new();
    private EditContext _editContext = default!;
    private PatientDto? _selectedPatient;
    private InsurancePolicyDto? _selectedPolicy;
    private ExaminationDto? _selectedExamination;

    protected override void OnInitialized()
        => _editContext = new EditContext(_model);

    private async Task<IEnumerable<PatientDto>> SearchPatientsAsync(string? value, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Array.Empty<PatientDto>();

        var page = await SafeExecute.RunAsync(
            () => PatientService.GetPagedAsync(value, "LastName", false, 1, 20, ct),
            Snackbar,
            () => T.PreAuthDialog.UnableSearchPatients);
        return page?.Items ?? Array.Empty<PatientDto>();
    }

    private async Task<IEnumerable<InsurancePolicyDto>> SearchPoliciesAsync(string? value, CancellationToken ct)
    {
        if (_selectedPatient is null)
        {
            Snackbar.Add(T.PreAuthDialog.SelectPatientFirst, Severity.Info);
            return Array.Empty<InsurancePolicyDto>();
        }

        var policies = await SafeExecute.RunAsync(
            () => InsuranceService.GetPoliciesByPatientAsync(_selectedPatient.Id, ct),
            Snackbar,
            () => T.PreAuthDialog.UnableSearchPolicies) ?? Array.Empty<InsurancePolicyDto>();

        if (string.IsNullOrWhiteSpace(value))
            return policies;

        return policies.Where(p => p.PolicyNumber.Contains(value, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<IEnumerable<ExaminationDto>> SearchExaminationsAsync(string? value, CancellationToken ct)
    {
        if (_selectedPatient is null)
        {
            Snackbar.Add(T.PreAuthDialog.SelectPatientFirst, Severity.Info);
            return Array.Empty<ExaminationDto>();
        }

        var page = await SafeExecute.RunAsync(
            () => ExaminationService.GetPagedAsync(value, "ScheduledAt", false, 1, 50, ct),
            Snackbar,
            () => T.PreAuthDialog.UnableSearchExaminations);
        return page?.Items.Where(e => e.PatientId == _selectedPatient.Id) ?? Array.Empty<ExaminationDto>();
    }

    private async Task SubmitAsync()
    {
        if (_selectedPatient is null || _selectedPolicy is null || _selectedExamination is null)
        {
            Snackbar.Add(T.PreAuthDialog.AllRequired, Severity.Error);
            return;
        }

        if (!_editContext.Validate())
            return;

        var input = new CreatePreAuthorizationInput
        {
            ExaminationId = _selectedExamination.Id,
            PatientId = _selectedPatient.Id,
            PolicyId = _selectedPolicy.Id,
            EstimatedAmount = _model.EstimatedAmount,
        };

        if (await TrySaveAsync(
                () => InsuranceService.CreatePreAuthorizationAsync(input),
                () => T.PreAuthDialog.UnreachableRetry))
        {
            Snackbar.Add(T.PreAuthDialog.Requested, Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
    }

    private sealed class PreAuthFormModel
    {
        [Range(0, double.MaxValue, ErrorMessage = "Estimated amount must be zero or greater.")]
        public decimal EstimatedAmount { get; set; }
    }
}
