using Microsoft.AspNetCore.Components;
using MudBlazor;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;
using RadiologyCenter.Desktop.Features.ReadingRoom.Components;
using RadiologyCenter.Desktop.Features.ReadingRoom.Models;

namespace RadiologyCenter.Desktop.Features.ReadingRoom.Pages;

public partial class ReadingRoom
{
    private async Task SelectExamAsync(QueueExam item)
    {
        _selected = item;
        _selectedSeries = 1;
        _loadingReport = true;
        _reportError = null;
        _report = null;
        StateHasChanged();
        try
        {
            await LoadReportForExamAsync(item);
        }
        catch (Exception)
        {
            _reportError = T.ReadingRoom.ReportLoadError;
        }
        finally
        {
            _loadingReport = false;
        }
    }

    private async Task ReloadSelectedAsync()
    {
        if (_selected is null)
            return;
        await SelectExamAsync(_selected);
    }

    private async Task RefreshQueueAsync()
    {
        await LoadQueueAsync();
        if (_selected is not null)
        {
            await SelectExamAsync(_selected);
        }
    }

    private async Task LoadReportForExamAsync(QueueExam item)
    {
        ReportDto? report;
        try
        {
            report = await ReportService.GetByExaminationAsync(item.Id);
        }
        catch (ApiException ex) when (ex.StatusCode == 404)
        {
            report = null;
        }

        _report = report;
        if (report is not null)
        {
            BuildEditor(report);
        }
        else
        {
            _sections.Clear();
        }
    }

    private async Task StartReportAsync()
    {
        if (_selected is null)
            return;

        if (string.IsNullOrWhiteSpace(_selected.AssignedRadiologistId))
        {
            var parameters = new DialogParameters<AssignStaffDialog>
            {
                { d => d.ExaminationId, _selected.Id },
                { d => d.ExaminationTypeId, _selected.ExaminationTypeId },
                { d => d.CurrentRadiologistId, _selected.AssignedRadiologistId },
                { d => d.CurrentTechnicianId, _selected.AssignedTechnicianId },
            };

            var dialog = await DialogService.ShowAsync<AssignStaffDialog>(string.Empty, parameters);
            var result = await dialog.Result;

            if (result is null || result.Canceled)
                return;

            if (result.Data is { } data)
            {
                var type = data.GetType();
                _selected.AssignedRadiologistId = type.GetProperty("RadiologistId")?.GetValue(data) as string;
                _selected.AssignedTechnicianId = type.GetProperty("TechnicianId")?.GetValue(data) as string;
            }
        }

        _loadingReport = true;
        _reportError = null;
        StateHasChanged();
        try
        {
            var report = await CreateDraftAsync(_selected);
            _report = report;
            BuildEditor(report);
        }
        catch (Exception)
        {
            _reportError = T.ReadingRoom.ReportLoadError;
        }
        finally
        {
            _loadingReport = false;
        }
    }

    private async Task<ReportDto> CreateDraftAsync(QueueExam item)
    {
        var draft = await ReportService.CreateDraftAsync(new CreateReportDraftInput
        {
            ExaminationId = item.Id,
            PatientId = item.PatientId,
            RadiologistId = item.AssignedRadiologistId ?? string.Empty,
        });

        if (!string.IsNullOrWhiteSpace(item.Indication))
        {
            draft = await ReportService.UpsertSectionAsync(draft.Id, new UpsertReportSectionInput
            {
                SectionType = "ClinicalIndication",
                Title = "Clinical Indication",
                Body = item.Indication,
                Position = 1,
                IsLocked = false,
            });
        }

        _reportStatusByExam[item.Id] = "Draft";
        _reportStatusKeyByExam[item.Id] = "Draft";
        item.ReportStatus = "Draft";
        item.ReportStatusKey = "Draft";
        return draft;
    }

    private void BuildEditor(ReportDto report)
    {
        _sections.Clear();
        var byType = (report.CurrentVersion?.Sections ?? Array.Empty<ReportSectionDto>())
            .GroupBy(s => s.SectionType)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

        foreach (var canonical in CanonicalTypes)
        {
            if (byType.TryGetValue(canonical.Type, out var section))
            {
                _sections.Add(new SectionEditor
                {
                    Type = section.SectionType,
                    Label = section.Title ?? GetCanonicalLabel(canonical.Type),
                    Body = section.Body ?? string.Empty,
                    Position = section.Position,
                    Locked = section.IsLocked,
                    Exists = true,
                });
            }
            else
            {
                _sections.Add(new SectionEditor
                {
                    Type = canonical.Type,
                    Label = GetCanonicalLabel(canonical.Type),
                    Body = string.Empty,
                    Position = canonical.Position,
                    Locked = false,
                    Exists = false,
                });
            }
        }

        // Include any server-defined sections that are not canonical.
        foreach (var section in (report.CurrentVersion?.Sections ?? Array.Empty<ReportSectionDto>())
                     .Where(s => !CanonicalTypes.Any(c => c.Type.Equals(s.SectionType, StringComparison.OrdinalIgnoreCase)))
                     .OrderBy(s => s.Position))
        {
            _sections.Add(new SectionEditor
            {
                Type = section.SectionType,
                Label = section.Title ?? section.SectionType,
                Body = section.Body ?? string.Empty,
                Position = section.Position,
                Locked = section.IsLocked,
                Exists = true,
            });
        }

        _sections.Sort((a, b) => a.Position.CompareTo(b.Position));

        _findings.Clear();
        _findings.AddRange(report.CurrentVersion?.Findings ?? Array.Empty<ReportFindingDto>());
    }

    private async Task SaveAsync()
    {
        if (!CanEdit || _report is null)
            return;

        await SafeExecute.RunAsync(
            async () =>
            {
                ReportDto? latest = null;
                var touched = 0;
                foreach (var section in _sections)
                {
                    if (section.Locked)
                        continue;

                    if (!section.Exists && string.IsNullOrWhiteSpace(section.Body))
                        continue;

                    var input = new UpsertReportSectionInput
                    {
                        SectionType = section.Type,
                        Title = section.Label,
                        Body = section.Body ?? string.Empty,
                        Position = section.Position,
                        IsLocked = false,
                    };
                    latest = await ReportService.UpsertSectionAsync(_report.Id, input);
                    touched++;
                }

                if (latest is not null)
                {
                    _report = latest;
                    BuildEditor(latest);
                    Snackbar.Add(T.ReadingRoom.DraftSaved, Severity.Success);
                }
            },
            Snackbar,
            () => T.ReadingRoom.Unreachable,
            busy => _busy = busy);
    }

    private async Task FinalizeAsync()
    {
        if (!CanEdit || _report is null)
            return;

        var parameters = new DialogParameters
        {
            ["Title"] = T.ReadingRoom.SignReport,
            ["Message"] = T.ReadingRoom.FinalizeConfirm,
            ["Icon"] = Icons.Material.Filled.EditNote,
            ["Color"] = Color.Primary,
            ["ConfirmText"] = T.ReadingRoom.SignAndFinalize,
            ["CancelText"] = T.ReadingRoom.Back,
        };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, options);
        var result = await dialog.Result;

        if (result?.Canceled != false)
            return;

        await SafeExecute.RunAsync(
            async () =>
            {
                _report = await ReportService.FinalizeAsync(_report.Id);
                BuildEditor(_report);
                if (_selected is not null)
                {
                    _selected.ReportStatus = "Finalized";
                    _selected.ReportStatusKey = "Finalized";
                    _reportStatusByExam[_selected.Id] = "Finalized";
                    _reportStatusKeyByExam[_selected.Id] = "Finalized";
                }
                Snackbar.Add(T.ReadingRoom.ReportSigned, Severity.Success);
            },
            Snackbar,
            () => T.ReadingRoom.Unreachable,
            busy => _busy = busy);
    }

    private async Task AddFindingAsync()
    {
        if (!CanEdit || _report is null)
            return;

        await SafeExecute.RunAsync(
            async () =>
            {
                await ReportService.AddFindingAsync(_report.Id, new AddReportFindingInput
                {
                    Region = _newRegion.Trim(),
                    Description = _newDescription.Trim(),
                    Severity = _newSeverity,
                });

                _newRegion = string.Empty;
                _newDescription = string.Empty;
                _newSeverity = "None";
                await ReloadReportAsync();
                Snackbar.Add(T.ReadingRoom.FindingAdded, Severity.Success);
            },
            Snackbar,
            () => T.ReadingRoom.Unreachable,
            busy => _busy = busy);
    }

    private async Task AddFindingFromListAsync((string Region, string Description, string Severity) args)
    {
        if (!CanEdit || _report is null)
            return;

        await SafeExecute.RunAsync(
            async () =>
            {
                await ReportService.AddFindingAsync(_report.Id, new AddReportFindingInput
                {
                    Region = args.Region,
                    Description = args.Description,
                    Severity = args.Severity,
                });

                await ReloadReportAsync();
                Snackbar.Add(T.ReadingRoom.FindingAdded, Severity.Success);
            },
            Snackbar,
            () => T.ReadingRoom.Unreachable,
            busy => _busy = busy);
    }

    private async Task OnSeverityChangedAsync(ReportFindingDto finding, string severity)
    {
        if (_report is null)
            return;

        await SafeExecute.RunAsync(
            async () =>
            {
                await ReportService.UpdateFindingAsync(_report.Id, finding.Id, new UpdateReportFindingInput
                {
                    Description = finding.Description,
                    Severity = severity,
                });
                await ReloadReportAsync();
                Snackbar.Add(T.ReadingRoom.FindingUpdated, Severity.Success);
            },
            Snackbar,
            () => T.ReadingRoom.Unreachable,
            busy => { });
    }

    private async Task OnSeverityChangedFromListAsync((ReportFindingDto Finding, string Severity) args)
        => await OnSeverityChangedAsync(args.Finding, args.Severity);

    private async Task RemoveFindingAsync(ReportFindingDto finding)
    {
        if (_report is null)
            return;

        var parameters = new DialogParameters
        {
            ["Title"] = T.ReadingRoom.RemoveFinding,
            ["Message"] = T.FormatValue(T.ReadingRoom.RemoveFindingConfirm, finding.Region),
            ["Icon"] = Icons.Material.Filled.Delete,
            ["Color"] = Color.Error,
            ["ConfirmText"] = T.ReadingRoom.Remove,
            ["CancelText"] = T.ReadingRoom.Keep,
        };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, options);
        var result = await dialog.Result;

        if (result?.Canceled != false)
            return;

        await SafeExecute.RunAsync(
            async () =>
            {
                await ReportService.RemoveFindingAsync(_report.Id, finding.Id);
                await ReloadReportAsync();
                Snackbar.Add(T.ReadingRoom.FindingRemoved, Severity.Success);
            },
            Snackbar,
            () => T.ReadingRoom.Unreachable,
            busy => { });
    }

    private async Task ReloadReportAsync()
    {
        if (_selected is null)
            return;
        var report = await ReportService.GetByExaminationAsync(_selected.Id);
        _report = report;
        BuildEditor(report);
    }

    private async Task OpenAmendAsync()
    {
        if (_report is null)
            return;

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        var parameters = new DialogParameters<AmendReportDialog>
        {
            { nameof(AmendReportDialog.ReportId), _report.Id },
            { nameof(AmendReportDialog.CurrentVersion), _report.CurrentVersionNumber },
        };

        var dialog = await DialogService.ShowAsync<AmendReportDialog>(T.AmendReport.Title, parameters, options);
        var result = await dialog.Result;
        if (result is { Canceled: false } && result.Data is ReportDto amended)
        {
            _report = amended;
            BuildEditor(amended);
            if (_selected is not null)
            {
                _selected.ReportStatus = "Draft";
                _selected.ReportStatusKey = "Draft";
            }
            Snackbar.Add(T.ReadingRoom.ReopenedForAmendment, Severity.Success);
        }
    }

    private async Task OpenCancelAsync()
    {
        if (_report is null)
            return;

        var parameters = new DialogParameters<CancelReportDialog>
        {
            { nameof(CancelReportDialog.ReportId), _report.Id },
        };

        var dialog = await DialogService.ShowAsync<CancelReportDialog>(T.CancelReport.Title, parameters,
            new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true });
        var result = await dialog.Result;
        if (result is { Canceled: false, Data: true })
        {
            await ReloadReportAsync();
            if (_selected is not null)
            {
                _selected.ReportStatus = "Cancelled";
                _selected.ReportStatusKey = "Cancelled";
            }
        }
    }

    private async Task OpenTemplatePicker()
    {
        if (_report is null)
            return;

        var dialog = await DialogService.ShowAsync<ReportTemplatePickerDialog>(T.ReportTemplate.Title,
            new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true });
        var result = await dialog.Result;
        if (result?.Canceled != false || result.Data is not string templateId || _report is null)
            return;

        await SafeExecute.RunAsync(
            async () =>
            {
                _report = await ReportService.ApplyTemplateAsync(_report.Id, templateId);
                BuildEditor(_report);
                Snackbar.Add(T.ReadingRoom.TemplateApplied, Severity.Success);
            },
            Snackbar,
            () => T.ReadingRoom.Unreachable,
            busy => { });
    }

    private async Task OpenVersionsAsync()
    {
        if (_report is null)
            return;

        var parameters = new DialogParameters<ReportVersionsDialog>
        {
            { nameof(ReportVersionsDialog.ReportId), _report.Id },
        };

        await DialogService.ShowAsync<ReportVersionsDialog>(T.ReportVersions.Title, parameters,
            new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true });
    }
}
