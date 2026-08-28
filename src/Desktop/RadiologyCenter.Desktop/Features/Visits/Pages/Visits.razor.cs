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

namespace RadiologyCenter.Desktop.Features.Visits.Pages;

public partial class Visits : ComponentBase, IDisposable
{
private MudTable<ExaminationDto>? _table;
    private string? _search;
    private CancellationTokenSource? _searchCts;
    private string? _loadError;
    private bool _offline;
    private readonly Dictionary<string, string> _patientNames = new();
    private readonly Dictionary<string, bool> _insuranceCache = new();

    private async Task PrimeInsuranceCacheAsync(IReadOnlyList<string> patientIds)
    {
        foreach (var id in patientIds)
        {
            if (_insuranceCache.ContainsKey(id))
                continue;
            try
            {
                var policies = await InsuranceService.GetPoliciesByPatientAsync(id);
                _insuranceCache[id] = policies.Count > 0;
            }
            catch
            {
                _insuranceCache[id] = false;
            }
        }
    }

    private bool HasInsurance(ExaminationDto visit)
        => _insuranceCache.TryGetValue(visit.PatientId, out var insured) && insured;

    private async Task<bool> ConfirmAsync(string message, string icon, MudBlazor.Color color)
    {
        var parameters = new DialogParameters
        {
            ["Title"] = T.Common.Confirm,
            ["Message"] = message,
            ["Icon"] = icon,
            ["Color"] = color,
            ["ConfirmText"] = T.Common.Confirm,
            ["CancelText"] = T.Common.Cancel,
        };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, options);
        var result = await dialog.Result;
        return result is { Canceled: false };
    }

    private async Task OpenInsuranceAsync(ExaminationDto visit)
    {
        if (!await ConfirmAsync(T.Visits.InsuranceConfirm, Icons.Material.Filled.Approval, MudBlazor.Color.Info))
            return;
        Navigation.NavigateTo("/insurance/preauthorizations");
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var page = await PatientService.GetPagedAsync(null, "LastName", false, 1, 1000);
            foreach (var patient in page.Items)
                _patientNames[patient.Id] = patient.FullName;
        }
        catch (Exception)
        {
            // patient names will fall back to the id
        }
    }

    private string ResolvePatientName(string patientId)
        => _patientNames.TryGetValue(patientId, out var name) ? name : "-";

    private async Task<TableData<ExaminationDto>> LoadServerData(TableState state, CancellationToken ct)
    {
        try
        {
            var page = await ExaminationService.GetPagedAsync(
                _search,
                state.SortLabel,
                state.SortDirection == SortDirection.Descending,
                state.Page + 1,
                state.PageSize,
                ct);

            await PrimeInsuranceCacheAsync(page.Items.Select(i => i.PatientId).Distinct().ToList());

            _loadError = null;
            _offline = false;
            return new TableData<ExaminationDto> { Items = page.Items, TotalItems = page.TotalCount };
        }
        catch (OperationCanceledException)
        {
            if (ct.IsCancellationRequested)
                return new TableData<ExaminationDto> { Items = Array.Empty<ExaminationDto>(), TotalItems = 0 };
            throw;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(SafeExecute.FormatError(ex), Severity.Error);
            _loadError = ex.Message;
            _offline = false;
            return new TableData<ExaminationDto> { Items = Array.Empty<ExaminationDto>(), TotalItems = 0 };
        }
        catch (Exception)
        {
            Snackbar.Add(T.Visits.Unreachable, Severity.Error);
            _loadError = T.Visits.Unreachable;
            _offline = true;
            return new TableData<ExaminationDto> { Items = Array.Empty<ExaminationDto>(), TotalItems = 0 };
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
        var dialog = await DialogService.ShowAsync<VisitEditorDialog>(T.Visits.NewVisit, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(ExaminationDto visit)
    {
        var parameters = new DialogParameters { ["Visit"] = visit };
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<VisitEditorDialog>(T.Visits.EditVisit, parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenScheduleDialogAsync(ExaminationDto visit)
    {
        if (!await ConfirmAsync(T.Visits.ScheduleConfirm, Icons.Material.Filled.Schedule, MudBlazor.Color.Info))
            return;
        var parameters = new DialogParameters { ["Visit"] = visit };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<VisitScheduleDialog>(T.Visits.ScheduleVisit, parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenViewDialogAsync(ExaminationDto visit)
    {
        var parameters = new DialogParameters { ["Visit"] = visit };
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<VisitViewDialog>(T.Visits.VisitDetails, parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task ReloadIfSavedAsync(IDialogReference dialog)
    {
        var result = await dialog.Result;
        if (result is { Canceled: false })
            await ReloadAsync();
    }

    private async Task CheckInAsync(ExaminationDto visit)
    {
        if (!await ConfirmAsync(T.Visits.CheckInConfirm, Icons.Material.Filled.Login, MudBlazor.Color.Primary))
            return;
        await SafeExecute.RunAsync(async () =>
            {
                await ExaminationService.CheckInAsync(visit.Id);
                Snackbar.Add(T.Visits.CheckedIn, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Visits.Unreachable);
    }

    private async Task StartAsync(ExaminationDto visit)
    {
        if (!await ConfirmAsync(T.Visits.StartConfirm, Icons.Material.Filled.PlayArrow, MudBlazor.Color.Secondary))
            return;
        await SafeExecute.RunAsync(async () =>
            {
                await ExaminationService.StartAsync(visit.Id);
                Snackbar.Add(T.Visits.Started, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Visits.Unreachable);
    }

    private async Task CompleteAsync(ExaminationDto visit)
    {
        if (!await ConfirmAsync(T.Visits.CompleteConfirm, Icons.Material.Filled.CheckCircle, MudBlazor.Color.Success))
            return;
        await SafeExecute.RunAsync(async () =>
            {
                await ExaminationService.CompleteAsync(visit.Id);
                Snackbar.Add(T.Visits.Completed, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Visits.Unreachable);
    }

    private async Task CancelAsync(ExaminationDto visit)
    {
        if (!await ConfirmAsync(T.Visits.CancelConfirm, Icons.Material.Filled.Cancel, MudBlazor.Color.Error))
            return;
        var parameters = new DialogParameters { ["Visit"] = visit };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<CancelVisitDialog>(T.Visits.CancelVisit, parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private static string FormatScheduled(DateTime? scheduledAt)
        => scheduledAt is null ? "-" : scheduledAt.Value.ToString("g");

    public void Dispose() => _searchCts?.Cancel();
}
