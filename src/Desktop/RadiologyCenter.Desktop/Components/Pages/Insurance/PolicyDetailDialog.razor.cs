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

public partial class PolicyDetailDialog : ComponentBase
{
[Parameter] public string PolicyId { get; set; } = string.Empty;
    [Parameter] public string? PatientName { get; set; }
    [Parameter] public string? CompanyName { get; set; }

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private InsurancePolicyDto? _policy;
    private IReadOnlyList<PolicyDocumentDto>? _documents;
    private string _patientName = string.Empty;
    private string _companyName = string.Empty;
    private string? _loadError;
    private bool _busy;

    protected override async Task OnInitializedAsync()
    {
        _patientName = PatientName ?? string.Empty;
        _companyName = CompanyName ?? string.Empty;
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        _loadError = null;
        try
        {
            _policy = await InsuranceService.GetPolicyByIdAsync(PolicyId);
            _documents = await InsuranceService.GetPolicyDocumentsAsync(PolicyId);
            _loadError = null;
        }
        catch (ApiException ex)
        {
            _loadError = ex.Message;
        }
        catch (Exception)
        {
            _loadError = T.PolicyDialog.Unreachable;
        }
    }

    private async Task OnFileSelectedAsync(InputFileChangeEventArgs e)
    {
        var file = e.File;
        if (file is null)
            return;

        await SafeExecute.RunAsync(async () =>
            {
                await using var stream = file.OpenReadStream(maxAllowedSize: 25 * 1024 * 1024);
                await InsuranceService.UploadPolicyDocumentAsync(
                    PolicyId,
                    "PolicyDocument",
                    file.Name,
                    file.ContentType,
                    stream);

                Snackbar.Add(T.PolicyDialog.DocumentUploaded, Severity.Success);
                await LoadAsync();
            },
            Snackbar,
            () => T.PolicyDialog.Unreachable,
            busy => _busy = busy);
    }

    private async Task DownloadDocumentAsync(PolicyDocumentDto document)
    {
        await SafeExecute.RunAsync(async () =>
            {
                var content = await InsuranceService.DownloadPolicyDocumentAsync(PolicyId, document.Id);
                await FileSaveHelper.SaveAsync(content, document.FileName);
                Snackbar.Add(T.PolicyDialog.DocumentDownloaded, Severity.Success);
            },
            Snackbar,
            () => T.PolicyDialog.UnableDownloadDocument);
    }

    private async Task DeleteDocumentAsync(PolicyDocumentDto document)
    {
        var parameters = new DialogParameters
        {
            ["Title"] = T.PolicyDialog.DeleteDocumentTitle,
            ["Message"] = T.FormatValue(T.PolicyDialog.DeleteDocumentConfirm, document.FileName),
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
                await InsuranceService.DeletePolicyDocumentAsync(PolicyId, document.Id);
                Snackbar.Add(T.PolicyDialog.DocumentDeleted, Severity.Success);
                await LoadAsync();
            },
            Snackbar,
            () => T.PolicyDialog.Unreachable);
    }

    private static string FormatDocumentMeta(PolicyDocumentDto document)
    {
        var kb = document.SizeInBytes / 1024m;
        return $"{document.Type} · {kb:0.#} KB · {document.UploadedAt:yyyy-MM-dd}";
    }

    private void CancelAsync()
        => MudDialog.Close();
}