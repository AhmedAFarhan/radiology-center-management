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

public partial class ClaimCreateDialog : ComponentBase
{
[CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private readonly ClaimFormModel _model = new();
    private EditContext _editContext = default!;
    private PatientDto? _selectedPatient;
    private InsurancePolicyDto? _selectedPolicy;
    private ExaminationDto? _selectedExamination;
    private PreAuthorizationDto? _selectedPreAuthorization;
    private bool _busy;

    protected override void OnInitialized()
        => _editContext = new EditContext(_model);

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
            Snackbar.Add(T.ClaimDialog.UnableSearchPatients, Severity.Error);
            return Array.Empty<PatientDto>();
        }
    }

    private async Task<IEnumerable<InsurancePolicyDto>> SearchPoliciesAsync(string? value, CancellationToken ct)
    {
        if (_selectedPatient is null)
        {
            Snackbar.Add(T.ClaimDialog.SelectPatientFirst, Severity.Info);
            return Array.Empty<InsurancePolicyDto>();
        }

        try
        {
            var policies = await InsuranceService.GetPoliciesByPatientAsync(_selectedPatient.Id, ct);
            if (string.IsNullOrWhiteSpace(value))
                return policies;

            return policies.Where(p => p.PolicyNumber.Contains(value, StringComparison.OrdinalIgnoreCase));
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            return Array.Empty<InsurancePolicyDto>();
        }
        catch (Exception)
        {
            Snackbar.Add(T.ClaimDialog.UnableSearchPolicies, Severity.Error);
            return Array.Empty<InsurancePolicyDto>();
        }
    }

    private async Task<IEnumerable<ExaminationDto>> SearchExaminationsAsync(string? value, CancellationToken ct)
    {
        if (_selectedPatient is null)
        {
            Snackbar.Add(T.ClaimDialog.SelectPatientFirst, Severity.Info);
            return Array.Empty<ExaminationDto>();
        }

        try
        {
            var page = await ExaminationService.GetPagedAsync(value, "ScheduledAt", false, 1, 50, ct);
            return page.Items.Where(e => e.PatientId == _selectedPatient.Id);
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            return Array.Empty<ExaminationDto>();
        }
        catch (Exception)
        {
            Snackbar.Add(T.ClaimDialog.UnableSearchExaminations, Severity.Error);
            return Array.Empty<ExaminationDto>();
        }
    }

    private async Task<IEnumerable<PreAuthorizationDto>> SearchPreAuthorizationsAsync(string? value, CancellationToken ct)
    {
        if (_selectedExamination is null)
        {
            Snackbar.Add(T.ClaimDialog.SelectExaminationFirst, Severity.Info);
            return Array.Empty<PreAuthorizationDto>();
        }

        try
        {
            var preAuthorization = await InsuranceService.GetPreAuthorizationByExaminationAsync(_selectedExamination.Id, ct);
            if (preAuthorization is null)
                return Array.Empty<PreAuthorizationDto>();

            return new[] { preAuthorization };
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            return Array.Empty<PreAuthorizationDto>();
        }
        catch (Exception)
        {
            Snackbar.Add(T.ClaimDialog.UnableSearchPreAuthorizations, Severity.Error);
            return Array.Empty<PreAuthorizationDto>();
        }
    }

    private async Task SubmitAsync()
    {
        if (_selectedPatient is null || _selectedPolicy is null || _selectedExamination is null || _selectedPreAuthorization is null)
        {
            Snackbar.Add(T.ClaimDialog.AllRequired, Severity.Error);
            return;
        }

        if (!_editContext.Validate())
            return;

        await SafeExecute.RunAsync(async () =>
            {
                var input = new CreateClaimInput
                {
                    ExaminationId = _selectedExamination.Id,
                    PatientId = _selectedPatient.Id,
                    PolicyId = _selectedPolicy.Id,
                    PreAuthorizationId = _selectedPreAuthorization.Id,
                    BilledAmount = _model.BilledAmount,
                };

                await InsuranceService.CreateClaimAsync(input);
                Snackbar.Add(T.ClaimDialog.Created, Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            },
            Snackbar,
            () => T.ClaimDialog.UnreachableRetry,
            busy => _busy = busy);
    }

    private void CancelAsync()
        => MudDialog.Cancel();

    private sealed class ClaimFormModel
    {
        [Range(0, double.MaxValue, ErrorMessage = "Billed amount must be zero or greater.")]
        public decimal BilledAmount { get; set; }
    }
}