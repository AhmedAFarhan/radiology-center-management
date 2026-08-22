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

public partial class ClaimDetailDialog : ComponentBase
{
[Parameter] public string ClaimId { get; set; } = string.Empty;
    [Parameter] public string? PatientName { get; set; }
    [Parameter] public string? ExaminationName { get; set; }
    [Parameter] public string? PolicyNumber { get; set; }

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private IReadOnlyList<EnumOptionDto> _settlementMethodOptions = Array.Empty<EnumOptionDto>();

    private ClaimDto? _claim;
    private string _patientName = string.Empty;
    private string _examinationName = string.Empty;
    private string _policyNumber = string.Empty;
    private decimal? _approvedAmount;
    private string? _rejectionCode;
    private string? _rejectionReason;
    private decimal _settlementAmount;
    private string _settlementMethod = string.Empty;
    private string? _settlementReference;
    private string? _loadError;
    private bool _busy;

    protected override async Task OnInitializedAsync()
    {
        _patientName = PatientName ?? string.Empty;
        _examinationName = ExaminationName ?? string.Empty;
        _policyNumber = PolicyNumber ?? string.Empty;
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        _loadError = null;
        try
        {
            _settlementMethodOptions = await EnumOptionsService.GetOptionsAsync("SettlementMethod");
            if (_settlementMethodOptions.Count > 0 && string.IsNullOrEmpty(_settlementMethod))
                _settlementMethod = _settlementMethodOptions[0].Key;

            _claim = await InsuranceService.GetClaimByIdAsync(ClaimId);
            _loadError = null;
        }
        catch (ApiException ex)
        {
            _loadError = ex.Message;
        }
        catch (Exception)
        {
            _loadError = T.ClaimDialog.Unreachable;
        }
    }

    private Task SubmitAsync()
        => SafeExecute.RunAsync(async () =>
        {
            _claim = await InsuranceService.SubmitClaimAsync(ClaimId);
            Snackbar.Add(T.ClaimDialog.Submitted, Severity.Success);
        }, Snackbar, () => T.ClaimDialog.Unreachable, busy => _busy = busy);

    private Task ApproveAsync()
        => SafeExecute.RunAsync(async () =>
        {
            var input = new AdjudicateClaimInput
            {
                Decision = "Approve",
                ApprovedAmount = _approvedAmount,
            };

            _claim = await InsuranceService.AdjudicateClaimAsync(ClaimId, input);
            Snackbar.Add(T.ClaimDialog.Approved, Severity.Success);
        }, Snackbar, () => T.ClaimDialog.Unreachable, busy => _busy = busy);

    private Task RejectAsync()
        => SafeExecute.RunAsync(async () =>
        {
            var input = new AdjudicateClaimInput
            {
                Decision = "Reject",
                RejectionCode = _rejectionCode,
                RejectionReason = _rejectionReason,
            };

            _claim = await InsuranceService.AdjudicateClaimAsync(ClaimId, input);
            Snackbar.Add(T.ClaimDialog.Rejected, Severity.Success);
        }, Snackbar, () => T.ClaimDialog.Unreachable, busy => _busy = busy);

    private Task ResubmitAsync()
        => SafeExecute.RunAsync(async () =>
        {
            _claim = await InsuranceService.ResubmitClaimAsync(ClaimId);
            Snackbar.Add(T.ClaimDialog.Resubmitted, Severity.Success);
        }, Snackbar, () => T.ClaimDialog.Unreachable, busy => _busy = busy);

    private Task RecordSettlementAsync()
        => SafeExecute.RunAsync(async () =>
        {
            var input = new RecordSettlementInput
            {
                Method = _settlementMethod,
                Amount = _settlementAmount,
                Reference = _settlementReference,
            };

            _claim = await InsuranceService.RecordSettlementAsync(ClaimId, input);
            Snackbar.Add(T.ClaimDialog.SettlementRecorded, Severity.Success);
        }, Snackbar, () => T.ClaimDialog.Unreachable, busy => _busy = busy);

    private void CancelAsync()
        => MudDialog.Close();
}