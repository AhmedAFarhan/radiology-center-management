using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;
using RadiologyCenter.Desktop.Components.Pages.Reports;
using Color = MudBlazor.Color;

namespace RadiologyCenter.Desktop.Components.Pages;

public partial class ReadingRoom : ComponentBase, IDisposable
{
[Inject] private ExaminationService ExaminationService { get; set; } = default!;
    [Inject] private PatientService PatientService { get; set; } = default!;
    [Inject] private ReportService ReportService { get; set; } = default!;
    [Inject] private PacsSyncService PacsSync { get; set; } = default!;
    [Inject] private IDialogService DialogService { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;
[Inject] private AppLocalizer T { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private EnumOptionsService EnumOptionsService { get; set; } = default!;
    private IReadOnlyList<EnumOptionDto> _severityOptions = Array.Empty<EnumOptionDto>();

    private readonly List<QueueExam> _queue = new();
    private readonly Dictionary<string, PatientDto> _patientCache = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _reportStatusByExam = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _reportStatusKeyByExam = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _linking = new(StringComparer.OrdinalIgnoreCase);

    private string? _filter;
    private bool _loadingQueue;
    private string? _queueError;
    private bool _loaded;

    private QueueExam? _selected;
    private bool _loadingReport;
    private string? _reportError;
    private ReportDto? _report;

    private int _selectedSeries = 1;

    private readonly List<SectionEditor> _sections = new();
    private readonly List<ReportFindingDto> _findings = new();

    private string _newRegion = string.Empty;
    private string _newDescription = string.Empty;
    private string _newSeverity = "None";
    private bool _busy;

    private bool _worklistCollapsed;
    private bool _reportCollapsed;
    private bool _viewerFullscreen;
    private DotNetObjectReference<ReadingRoom>? _jsRef;

    private void ToggleWorklist() => _worklistCollapsed = !_worklistCollapsed;

    private void ToggleReport() => _reportCollapsed = !_reportCollapsed;

    private void ToggleViewerFullscreen() => _viewerFullscreen = !_viewerFullscreen;

    [JSInvokable]
    public void OnViewerEscape()
    {
        if (_viewerFullscreen)
        {
            _viewerFullscreen = false;
            StateHasChanged();
        }
    }

    private record CanonicalSection(string Type, string Label, int Position);

    private static readonly CanonicalSection[] CanonicalTypes =
    {
        new("ClinicalIndication", "Clinical Indication", 1),
        new("Technique", "Technique", 2),
        new("Findings", "Findings", 3),
        new("Impression", "Impression / Conclusion", 4),
        new("Recommendation", "Recommendation", 5),
    };

    private IEnumerable<QueueExam> Queue
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_filter))
                return _queue;

            var f = _filter.Trim();
            return _queue.Where(q =>
                q.PatientName.Contains(f, StringComparison.OrdinalIgnoreCase) ||
                q.ExamName.Contains(f, StringComparison.OrdinalIgnoreCase) ||
                q.PatientCode.Contains(f, StringComparison.OrdinalIgnoreCase) ||
                q.Priority.Contains(f, StringComparison.OrdinalIgnoreCase) ||
                q.ReportStatus.Contains(f, StringComparison.OrdinalIgnoreCase));
        }
    }

    private int PendingCount => _queue.Count(q => q.ReportStatusKey is "New" or "Draft");

    private int ImagedCount => _queue.Count(q => q.StudyInstanceUid is not null);

    private bool CanEdit => _report is { StatusKey: "Draft" };

    private QueueExam? Selected => _selected;

    private static int SeriesCount => 4;

    private string? SelectedStudyUrl
    {
        get
        {
            if (_selected is not { StudyInstanceUid.Length: > 0 } selected)
                return null;
            var baseUrl = PacsService.Instance?.ViewerBaseUrl;
            return baseUrl is null ? null : $"{baseUrl}/viewer?StudyInstanceUIDs={Uri.EscapeDataString(selected.StudyInstanceUid)}";
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender && !_loaded)
        {
            _loaded = true;
            _jsRef = DotNetObjectReference.Create(this);
            await JS.InvokeVoidAsync("readingRoomKeydown", _jsRef);
            _ = LoadSeverityOptionsAsync();
            _ = LoadQueueAsync();
        }
    }

    private async Task LoadSeverityOptionsAsync()
    {
        try
        {
            _severityOptions = await EnumOptionsService.GetOptionsAsync("FindingSeverity");
        }
        catch
        {
            // severity options are non-critical; leave empty
        }
    }

    public void Dispose()
    {
        _ = JS.InvokeVoidAsync("unregisterReadingRoomKeydown");
        _jsRef?.Dispose();
    }

    private bool IsSelected(QueueExam exam) => _selected?.Id == exam.Id;

    private async Task LoadQueueAsync()
    {
        _loadingQueue = true;
        _queueError = null;
        try
        {
            var exams = await ExaminationService.GetPagedAsync(null, null, false, 1, 100);
            var visible = exams.Items
                .Where(e => e.StatusKey is "CheckedIn" or "Completed")
                .ToList();

            IReadOnlyList<ReportListItemDto> reportItems = Array.Empty<ReportListItemDto>();
            try
            {
                var reports = await ReportService.GetPagedAsync(null, null, false, 1, 100);
                reportItems = reports.Items;
            }
            catch (Exception)
            {
                // report status badges are best-effort
            }

            _reportStatusByExam.Clear();
            _reportStatusKeyByExam.Clear();
            foreach (var report in reportItems)
            {
                _reportStatusByExam[report.ExaminationId] = report.Status;
                _reportStatusKeyByExam[report.ExaminationId] = report.StatusKey;
            }

            _queue.Clear();
            foreach (var exam in visible)
            {
                var patient = await GetPatientAsync(exam.PatientId);
                var patientCode = patient?.PatientCode;
                _queue.Add(new QueueExam
                {
                    Id = exam.Id,
                    PatientId = exam.PatientId,
                    PatientName = patient?.FullName ?? $"Patient {exam.PatientId}",
                    PatientCode = patientCode ?? "-",
                    ExamName = exam.ExaminationTypeName ?? "Examination",
                    StatusKey = exam.StatusKey,
                    ScheduledAt = exam.ScheduledAt,
                    CompletedAt = exam.CompletedAt,
                    Priority = exam.Priority,
                    Indication = exam.ClinicalIndication,
                    AssignedRadiologistId = exam.RadiologistId,
                    ReportStatus = _reportStatusByExam.GetValueOrDefault(exam.Id, "New"),
                    ReportStatusKey = _reportStatusKeyByExam.GetValueOrDefault(exam.Id, "New"),
                    StudyInstanceUid = string.IsNullOrEmpty(exam.StudyInstanceUID) ? null : exam.StudyInstanceUID,
                });
            }

            _queue.Sort((a, b) =>
            {
                // CheckedIn first, then Completed; within each group by their own date.
                var groupCompare = (a.StatusKey == "CheckedIn" ? 0 : 1)
                    .CompareTo(b.StatusKey == "CheckedIn" ? 0 : 1);
                if (groupCompare != 0)
                    return groupCompare;
                return a.StatusKey == "CheckedIn"
                    ? (a.ScheduledAt ?? DateTime.MaxValue).CompareTo(b.ScheduledAt ?? DateTime.MaxValue)
                    : (b.CompletedAt ?? DateTime.MinValue).CompareTo(a.CompletedAt ?? DateTime.MinValue);
            });
        }
        catch (Exception)
        {
            _queueError = T.ReadingRoom.Unreachable;
        }
        finally
        {
            _loadingQueue = false;
        }
    }

    private async Task<PatientDto?> GetPatientAsync(string patientId)
    {
        if (_patientCache.TryGetValue(patientId, out var cached))
            return cached;
        try
        {
            var patient = await PatientService.GetByIdAsync(patientId);
            _patientCache[patientId] = patient;
            return patient;
        }
        catch
        {
            return null;
        }
    }

    private void OnFilterChanged(string? value)
        => _filter = value;

    private bool IsLinking(QueueExam exam) => _linking.Contains(exam.Id);

    private async Task LinkImagesAsync(QueueExam item)
    {
        if (IsLinking(item) || item.StudyInstanceUid is not null)
            return;

        _linking.Add(item.Id);
        StateHasChanged();
        try
        {
            var linkedUid = await PacsSync.LinkStudyToExamAsync(
                item.Id,
                item.PatientCode,
                accessionNumber: null,
                item.PatientName);

            if (linkedUid is null)
            {
                Snackbar.Add(T.ReadingRoom.NoMatchingStudy, Severity.Warning);
                return;
            }

            await LoadQueueAsync();
            var refreshed = _queue.FirstOrDefault(q => q.Id == item.Id);
            if (refreshed is not null)
                await SelectExamAsync(refreshed);
            Snackbar.Add(T.ReadingRoom.ImagesLinked, Severity.Success);
        }
        catch (Exception)
        {
            Snackbar.Add(T.ReadingRoom.Unreachable, Severity.Error);
        }
        finally
        {
            _linking.Remove(item.Id);
        }
    }

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
                    Label = section.Title ?? canonical.Label,
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
                    Label = canonical.Label,
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

        _busy = true;
        try
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
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        catch (Exception)
        {
            Snackbar.Add(T.ReadingRoom.Unreachable, Severity.Error);
        }
        finally
        {
            _busy = false;
        }
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

        _busy = true;
        try
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
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        catch (Exception)
        {
            Snackbar.Add(T.ReadingRoom.Unreachable, Severity.Error);
        }
        finally
        {
            _busy = false;
        }
    }

    private async Task AddFindingAsync()
    {
        if (!CanEdit || _report is null)
            return;

        _busy = true;
        try
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
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        catch (Exception)
        {
            Snackbar.Add(T.ReadingRoom.Unreachable, Severity.Error);
        }
        finally
        {
            _busy = false;
        }
    }

    private async Task OnSeverityChangedAsync(ReportFindingDto finding, string severity)
    {
        if (_report is null)
            return;

        try
        {
            await ReportService.UpdateFindingAsync(_report.Id, finding.Id, new UpdateReportFindingInput
            {
                Description = finding.Description,
                Severity = severity,
            });
            await ReloadReportAsync();
            Snackbar.Add(T.ReadingRoom.FindingUpdated, Severity.Success);
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        catch (Exception)
        {
            Snackbar.Add(T.ReadingRoom.Unreachable, Severity.Error);
        }
    }

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

        try
        {
            await ReportService.RemoveFindingAsync(_report.Id, finding.Id);
            await ReloadReportAsync();
            Snackbar.Add(T.ReadingRoom.FindingRemoved, Severity.Success);
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        catch (Exception)
        {
            Snackbar.Add(T.ReadingRoom.Unreachable, Severity.Error);
        }
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

        try
        {
            _report = await ReportService.ApplyTemplateAsync(_report.Id, templateId);
            BuildEditor(_report);
            Snackbar.Add(T.ReadingRoom.TemplateApplied, Severity.Success);
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        catch (Exception)
        {
            Snackbar.Add(T.ReadingRoom.Unreachable, Severity.Error);
        }
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

    private static string PriorityClass(string priority) => priority switch
    {
        "Stat" => "stat",
        "Urgent" => "urgent",
        _ => "routine",
    };

    private static string Initials(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "?";
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return string.Concat(parts.Take(2).Select(p => p[0])).ToUpperInvariant();
    }

    private sealed class QueueExam
    {
        public string Id { get; init; } = string.Empty;
        public string PatientId { get; init; } = string.Empty;
        public string PatientName { get; init; } = string.Empty;
        public string PatientCode { get; init; } = string.Empty;
        public string ExamName { get; init; } = string.Empty;
        public string StatusKey { get; init; } = "Completed";
        public DateTime? ScheduledAt { get; init; }
        public DateTime? CompletedAt { get; init; }
        public string Priority { get; init; } = string.Empty;
        public string Indication { get; init; } = string.Empty;
        public string? AssignedRadiologistId { get; init; }
        public string ReportStatus { get; set; } = "New";
        public string ReportStatusKey { get; set; } = "New";
        public string? StudyInstanceUid { get; init; }

        public string DateLabel => (StatusKey == "CheckedIn" ? ScheduledAt : CompletedAt)?.ToString("yyyy-MM-dd") ?? string.Empty;
    }

    private sealed class SectionEditor
    {
        public string Type { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public int Position { get; set; }
        public bool Locked { get; set; }
        public bool Exists { get; set; }
    }
}