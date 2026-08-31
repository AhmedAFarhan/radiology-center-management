using Microsoft.AspNetCore.Components;
using MudBlazor;
using RadiologyCenter.Desktop.Features.Examinations.Models;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;
using RadiologyCenter.Desktop.Shared.Components;

namespace RadiologyCenter.Desktop.Features.Visits.Pages;

public partial class Visits : ListPageBase<ExaminationListItemDto>
{
    [Inject] private ExaminationService ExaminationService { get; set; } = default!;
    [Inject] private PatientService PatientService { get; set; } = default!;
    [Inject] private InsuranceService InsuranceService { get; set; } = default!;

    protected override string BaseRoute => "/visits";

    protected override string UnreachableMessage => T.Visits.Unreachable;

    private readonly Dictionary<string, string> _patientNames = new();
    private readonly Dictionary<string, bool> _insuranceCache = new();

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

    protected override async Task<PagedResult<ExaminationListItemDto>> LoadPageAsync(
        string? search,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        var result = await ExaminationService.GetPagedAsync(search, sortBy, sortDescending, page, pageSize, ct);
        await PrimeInsuranceCacheAsync(result.Items.Select(i => i.PatientId).Distinct().ToList());
        return result;
    }

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

    private bool HasInsurance(ExaminationListItemDto visit)
        => _insuranceCache.TryGetValue(visit.PatientId, out var insured) && insured;

    private string ResolvePatientName(string patientId)
        => _patientNames.TryGetValue(patientId, out var name) ? name : "-";

    private static string FormatScheduled(DateTime? scheduledAt)
        => scheduledAt is null ? "-" : scheduledAt.Value.ToString("g");

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

    private async Task<ExaminationDto?> LoadDetailAsync(string id)
    {
        ExaminationDto? detail = null;
        await SafeExecute.RunAsync(
            async () => { detail = await ExaminationService.GetByIdAsync(id); },
            Snackbar,
            () => T.Visits.Unreachable);
        return detail;
    }

    private async Task OpenCreateDialogAsync()
    {
        var dialog = await DialogService.ShowAsync<VisitEditorDialog>(T.Visits.NewVisit, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(ExaminationListItemDto visit)
    {
        var detail = await LoadDetailAsync(visit.Id);
        if (detail is null) return;
        var parameters = new DialogParameters { ["Visit"] = detail };
        var dialog = await DialogService.ShowAsync<VisitEditorDialog>(T.Visits.EditVisit, parameters, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenScheduleDialogAsync(ExaminationListItemDto visit)
    {
        if (!await ConfirmAsync(T.Visits.ScheduleConfirm, Icons.Material.Filled.Schedule, MudBlazor.Color.Info))
            return;
        var detail = await LoadDetailAsync(visit.Id);
        if (detail is null) return;
        var parameters = new DialogParameters { ["Visit"] = detail };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<VisitScheduleDialog>(T.Visits.ScheduleVisit, parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenViewDialogAsync(ExaminationListItemDto visit)
    {
        var detail = await LoadDetailAsync(visit.Id);
        if (detail is null) return;
        var parameters = new DialogParameters { ["Visit"] = detail };
        var dialog = await DialogService.ShowAsync<VisitViewDialog>(T.Visits.VisitDetails, parameters, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenInsuranceAsync(ExaminationListItemDto visit)
    {
        if (!await ConfirmAsync(T.Visits.InsuranceConfirm, Icons.Material.Filled.Approval, MudBlazor.Color.Info))
            return;
        NavigationManager.NavigateTo("/insurance/preauthorizations");
    }

    private async Task CheckInAsync(ExaminationListItemDto visit)
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

    private async Task StartAsync(ExaminationListItemDto visit)
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

    private async Task CompleteAsync(ExaminationListItemDto visit)
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

    private async Task CancelAsync(ExaminationListItemDto visit)
    {
        if (!await ConfirmAsync(T.Visits.CancelConfirm, Icons.Material.Filled.Cancel, MudBlazor.Color.Error))
            return;
        var detail = await LoadDetailAsync(visit.Id);
        if (detail is null) return;
        var parameters = new DialogParameters { ["Visit"] = detail };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<CancelVisitDialog>(T.Visits.CancelVisit, parameters, options);
        await ReloadIfSavedAsync(dialog);
    }
}
