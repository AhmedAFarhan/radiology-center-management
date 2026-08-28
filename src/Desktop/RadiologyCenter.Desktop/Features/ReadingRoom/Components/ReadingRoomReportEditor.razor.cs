using Microsoft.AspNetCore.Components;
using MudBlazor;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;
using RadiologyCenter.Desktop.Features.ReadingRoom.Models;
using RadiologyCenter.Desktop.Features.Reports.Components;

namespace RadiologyCenter.Desktop.Features.ReadingRoom.Components;

public partial class ReadingRoomReportEditor : ComponentBase
{
    [Parameter] public ReportDto? Report { get; set; }
    [Parameter] public bool Loading { get; set; }
    [Parameter] public string? Error { get; set; }
    [Parameter] public bool CanEdit { get; set; }
    [Parameter] public bool Busy { get; set; }
    [Parameter] public string PatientName { get; set; } = string.Empty;
    [Parameter] public string PatientCode { get; set; } = string.Empty;
    [Parameter] public string ExamName { get; set; } = string.Empty;
    [Parameter] public string Priority { get; set; } = string.Empty;
    [Parameter] public string? Indication { get; set; }
    [Parameter] public IReadOnlyList<SectionEditor> Sections { get; set; } = Array.Empty<SectionEditor>();
    [Parameter] public IReadOnlyList<ReportFindingDto> Findings { get; set; } = Array.Empty<ReportFindingDto>();
    [Parameter] public IReadOnlyList<EnumOptionDto> SeverityOptions { get; set; } = Array.Empty<EnumOptionDto>();

    [Parameter] public EventCallback OnRetry { get; set; }
    [Parameter] public EventCallback OnSave { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }
    [Parameter] public EventCallback OnFinalize { get; set; }
    [Parameter] public EventCallback OnAmend { get; set; }
    [Parameter] public EventCallback<(ReportFindingDto Finding, string Severity)> OnSeverityChanged { get; set; }
    [Parameter] public EventCallback<ReportFindingDto> OnRemoveFinding { get; set; }
    [Parameter] public EventCallback<(string Region, string Description, string Severity)> OnAddFinding { get; set; }

    [Inject] private IDialogService DialogService { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;
    [Inject] private AppLocalizer T { get; set; } = default!;
    [Inject] private ReportService ReportService { get; set; } = default!;

    private static string Initials(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "?";
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return string.Concat(parts.Take(2).Select(p => p[0])).ToUpperInvariant();
    }

    private async Task OpenTemplatePicker()
    {
        if (Report is null)
            return;

        var dialog = await DialogService.ShowAsync<ReportTemplatePickerDialog>(T.ReportTemplate.Title,
            new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true });
        var result = await dialog.Result;
        if (result?.Canceled != false || result.Data is not string templateId || Report is null)
            return;

        try
        {
            var updated = await ReportService.ApplyTemplateAsync(Report.Id, templateId);
            Report = updated;
            await OnRetry.InvokeAsync();
            Snackbar.Add(T.ReadingRoom.TemplateApplied, Severity.Success);
        }
        catch (ApiException ex)
        {
            Snackbar.Add(SafeExecute.FormatError(ex), Severity.Error);
        }
        catch (Exception)
        {
            Snackbar.Add(T.ReadingRoom.Unreachable, Severity.Error);
        }
    }

    private async Task OpenVersionsAsync()
    {
        if (Report is null)
            return;

        var parameters = new DialogParameters<ReportVersionsDialog>
        {
            { nameof(ReportVersionsDialog.ReportId), Report.Id },
        };

        await DialogService.ShowAsync<ReportVersionsDialog>(T.ReportVersions.Title, parameters,
            new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true });
    }
}
