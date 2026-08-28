using Microsoft.AspNetCore.Components;
using MudBlazor;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;
using RadiologyCenter.Desktop.Shared.Components;

namespace RadiologyCenter.Desktop.Features.Patients.Pages;

public partial class Patients : ListPageBase<PatientDto>
{
    [Inject] private PatientService PatientService { get; set; } = default!;

    protected override string BaseRoute => "/patients";

    protected override string UnreachableMessage => T.Patients.Unreachable;

    protected override async Task<PagedResult<PatientDto>> LoadPageAsync(
        string? search,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken ct)
        => await PatientService.GetPagedAsync(search, sortBy, sortDescending, page, pageSize, ct);

    protected override async Task OpenByDeepLinkAsync(string id)
    {
        PatientDto? patient = null;
        var ok = await SafeExecute.RunAsync(
            async () => { patient = await PatientService.GetByIdAsync(id); },
            Snackbar,
            () => T.Patients.Unreachable);

        if (ok && patient is not null)
        {
            var parameters = new DialogParameters { ["Patient"] = patient };
            var dialog = await DialogService.ShowAsync<PatientEditorDialog>(T.Patients.EditPatient, parameters, EditorDialogOptions);
            await ReloadIfSavedAsync(dialog);
        }

        NavigationManager.NavigateTo(BaseRoute, replace: true);
    }

    private async Task ExportToExcelAsync()
    {
        await SafeExecute.RunAsync(
            async () =>
            {
                var content = await PatientService.ExportAsync(Search);
                var path = await FileSaveHelper.SaveAsync(content, "patients.xlsx");
                Snackbar.Add(T.FormatValue(T.Common.SavedTo, path), Severity.Success);
            },
            Snackbar,
            () => T.Patients.Unreachable);
    }

    private async Task OpenImportDialogAsync()
    {
        var parameters = new DialogParameters
        {
            ["DownloadTemplate"] = new Func<Task<byte[]>>(() => PatientService.DownloadImportTemplateAsync()),
            ["ImportFile"] = new Func<string, Stream, Task<ExcelImportResultDto>>((fileName, stream) => PatientService.ImportAsync(fileName, stream)),
            ["Imported"] = EventCallback.Factory.Create(this, ReloadAsync),
        };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader = true };
        await DialogService.ShowAsync<ExcelImportDialog>(string.Empty, parameters, options);
    }

    private async Task OpenCreateDialogAsync()
    {
        var dialog = await DialogService.ShowAsync<PatientEditorDialog>(T.Patients.NewPatient, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(PatientDto patient)
    {
        var parameters = new DialogParameters { ["Patient"] = patient };
        var dialog = await DialogService.ShowAsync<PatientEditorDialog>(T.Patients.EditPatient, parameters, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
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
}
