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
using RadiologyCenter.Desktop.Features.Insurance.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Features.Insurance.Components;

public partial class PreAuthDetailDialog : ComponentBase
{
[Parameter] public PreAuthorizationListItemDto? PreAuthorization { get; set; }

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private PreAuthorizationDto? _preAuth;
    private IReadOnlyList<PreAuthorizationDocumentDto>? _documents;
    private decimal? _approvedAmount;
    private string? _rejectionReason;
    private string? _loadError;
    private bool _busy;

    private string Id => PreAuthorization?.Id ?? string.Empty;
    private string _patientName => PreAuthorization?.PatientName ?? string.Empty;
    private string _examinationName => PreAuthorization?.ExaminationTypeName ?? string.Empty;
    private string _policyNumber => PreAuthorization?.PolicyNumber ?? string.Empty;

    protected override async Task OnInitializedAsync()
    {
        _preAuth = PreAuthorization is null
            ? null
            : new PreAuthorizationDto(
                PreAuthorization.Id,
                PreAuthorization.ExaminationId,
                PreAuthorization.PatientId,
                PreAuthorization.PolicyId,
                PreAuthorization.EstimatedAmount,
                PreAuthorization.Status,
                PreAuthorization.RequestedAt,
                PreAuthorization.DecidedAt,
                PreAuthorization.ApprovedAmount,
                PreAuthorization.RejectionReason,
                PreAuthorization.IsGovernment,
                StatusKey: PreAuthorization.StatusKey);
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        _loadError = null;
        try
        {
            _documents = await InsuranceService.GetPreAuthorizationDocumentsAsync(Id);
            _loadError = null;
        }
        catch (ApiException ex)
        {
            _loadError = ex.Message;
        }
        catch (Exception)
        {
            _loadError = T.PreAuthDialog.Unreachable;
        }
    }

    private Task ApproveAsync()
        => SafeExecute.RunAsync(async () =>
        {
            var input = new DecidePreAuthorizationInput
            {
                Decision = "Approve",
                ApprovedAmount = _approvedAmount,
            };

            _preAuth = await InsuranceService.DecidePreAuthorizationAsync(Id, input);
            Snackbar.Add(T.PreAuthDialog.Approved, Severity.Success);
            await LoadAsync();
        }, Snackbar, () => T.PreAuthDialog.Unreachable, busy => _busy = busy);

    private Task DenyAsync()
        => SafeExecute.RunAsync(async () =>
        {
            var input = new DecidePreAuthorizationInput
            {
                Decision = "Deny",
                RejectionReason = _rejectionReason,
            };

            _preAuth = await InsuranceService.DecidePreAuthorizationAsync(Id, input);
            Snackbar.Add(T.PreAuthDialog.Denied, Severity.Success);
            await LoadAsync();
        }, Snackbar, () => T.PreAuthDialog.Unreachable, busy => _busy = busy);

    private async Task OnFileSelectedAsync(InputFileChangeEventArgs e)
    {
        var file = e.File;
        if (file is null)
            return;

        await SafeExecute.RunAsync(async () =>
            {
                await using var stream = file.OpenReadStream(maxAllowedSize: 25 * 1024 * 1024);
                await InsuranceService.UploadPreAuthorizationDocumentAsync(
                    Id,
                    "SupportingDocument",
                    file.Name,
                    file.ContentType,
                    stream);

                Snackbar.Add(T.PreAuthDialog.DocumentUploaded, Severity.Success);
                await LoadAsync();
            },
            Snackbar,
            () => T.PreAuthDialog.Unreachable,
            busy => _busy = busy);
    }

    private async Task DownloadDocumentAsync(PreAuthorizationDocumentDto document)
    {
        await SafeExecute.RunAsync(async () =>
            {
                var content = await InsuranceService.DownloadPreAuthorizationDocumentAsync(Id, document.Id);
                await FileSaveHelper.SaveAsync(content, document.FileName);
                Snackbar.Add(T.PreAuthDialog.DocumentDownloaded, Severity.Success);
            },
            Snackbar,
            () => T.PreAuthDialog.UnableDownloadDocument);
    }

    private async Task DeleteDocumentAsync(PreAuthorizationDocumentDto document)
    {
        var parameters = new DialogParameters
        {
            ["Title"] = T.PreAuthDialog.DeleteDocumentTitle,
            ["Message"] = T.FormatValue(T.PreAuthDialog.DeleteDocumentConfirm, document.FileName),
            ["Icon"] = Icons.Material.Filled.Delete,
            ["Color"] = MudBlazor.Color.Error,
            ["ConfirmText"] = T.Common.Delete,
            ["CancelText"] = T.Common.Cancel,
        };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, options);
        var result = await dialog.Result;

        if (result?.Canceled != false)
            return;

        await SafeExecute.RunAsync(async () =>
            {
                await InsuranceService.DeletePreAuthorizationDocumentAsync(Id, document.Id);
                Snackbar.Add(T.PreAuthDialog.DocumentDeleted, Severity.Success);
                await LoadAsync();
            },
            Snackbar,
            () => T.PreAuthDialog.Unreachable);
    }

    private void CancelAsync()
        => MudDialog.Close();
}
