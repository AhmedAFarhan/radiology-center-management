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

namespace RadiologyCenter.Desktop.Components.Pages.Patients;

public partial class Patients : ComponentBase, IDisposable
{
private MudTable<PatientDto>? _table;
    private string? _search;
    private CancellationTokenSource? _searchCts;
    private string? _loadError;
    private bool _offline;
    private string? _openPatientId;

    [SupplyParameterFromQuery(Name = "q")]
    public string? SearchQuery { get; set; }

    [SupplyParameterFromQuery(Name = "open")]
    public string? OpenId { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (!string.IsNullOrWhiteSpace(OpenId) && Guid.TryParse(OpenId, out _))
            _openPatientId = OpenId;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_openPatientId is not null)
        {
            var id = _openPatientId;
            _openPatientId = null;
            await OpenPatientAsync(id);
        }
    }

    private async Task OpenPatientAsync(string id)
    {
        PatientDto? patient = null;
        var ok = await SafeExecute.RunAsync(
            async () => patient = await PatientService.GetByIdAsync(id),
            Snackbar,
            () => T.Patients.Unreachable);

        if (ok && patient is not null)
        {
            var parameters = new DialogParameters { ["Patient"] = patient };
            var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
            var dialog = await DialogService.ShowAsync<PatientEditorDialog>(T.Patients.EditPatient, parameters, options);
            await ReloadIfSavedAsync(dialog);
        }

        NavigationManager.NavigateTo("/patients", replace: true);
    }

    private async Task<TableData<PatientDto>> LoadServerData(TableState state, CancellationToken ct)
    {
        try
        {
            var page = await PatientService.GetPagedAsync(
                _search,
                state.SortLabel,
                state.SortDirection == SortDirection.Descending,
                state.Page + 1,
                state.PageSize,
                ct);

            _loadError = null;
            _offline = false;
            return new TableData<PatientDto> { Items = page.Items, TotalItems = page.TotalCount };
        }
        catch (OperationCanceledException)
        {
            if (ct.IsCancellationRequested)
                return new TableData<PatientDto> { Items = Array.Empty<PatientDto>(), TotalItems = 0 };
            throw;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            _loadError = ex.Message;
            _offline = false;
            return new TableData<PatientDto> { Items = Array.Empty<PatientDto>(), TotalItems = 0 };
        }
        catch (Exception)
        {
            Snackbar.Add(T.Patients.Unreachable, Severity.Error);
            _loadError = T.Patients.Unreachable;
            _offline = true;
            return new TableData<PatientDto> { Items = Array.Empty<PatientDto>(), TotalItems = 0 };
        }
    }

    private async Task OnSearchChanged(string? value)
    {
        _search = value;

        _searchCts?.Cancel();
        var cts = _searchCts = new CancellationTokenSource();
        try
        {
            await Task.Delay(400, cts.Token);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        if (_table is not null)
            await _table.ReloadServerData();
    }

    private Task ReloadAsync()
        => _table is null ? Task.CompletedTask : _table.ReloadServerData();

    private async Task OpenCreateDialogAsync()
    {
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<PatientEditorDialog>(T.Patients.NewPatient, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(PatientDto patient)
    {
        var parameters = new DialogParameters { ["Patient"] = patient };
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<PatientEditorDialog>(T.Patients.EditPatient, parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task ReloadIfSavedAsync(IDialogReference dialog)
    {
        var result = await dialog.Result;
        if (result is { Canceled: false })
            await ReloadAsync();
    }

    private async Task ToggleActiveAsync(PatientDto patient)
    {
        if (!await ConfirmDialogs.ConfirmStatusChangeAsync(DialogService, T, T.Patients.ToggleStatus, patient.FullName, !patient.IsActive))
            return;

        await SafeExecute.RunAsync(async () =>
            {
                if (patient.IsActive)
                    await PatientService.DeactivateAsync(patient.Id);
                else
                    await PatientService.ActivateAsync(patient.Id);

                Snackbar.Add(patient.IsActive ? T.Patients.Deactivated : T.Patients.Activated, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Patients.Unreachable);
    }

    private async Task DeletePatientAsync(PatientDto patient)
    {
        var parameters = new DialogParameters
        {
            ["Title"] = T.Patients.DeleteTitle,
            ["Message"] = T.FormatValue(T.Patients.DeleteConfirm, patient.FullName, patient.PatientCode),
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
                await PatientService.DeleteAsync(patient.Id);
                Snackbar.Add(T.Patients.Deleted, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Patients.Unreachable);
    }

    private static string FormatAge(int? age, DateTime? dateOfBirth)
    {
        if (age is not null)
            return age.Value.ToString();

        if (dateOfBirth is not null)
        {
            var today = DateTime.UtcNow.Date;
            var birth = dateOfBirth.Value.Date;
            var years = today.Year - birth.Year;
            if (birth > today.AddYears(-years))
                years--;
            return years.ToString();
        }

        return "-";
    }

    public void Dispose() => _searchCts?.Cancel();
}