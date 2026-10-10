using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;
using RadiologyCenter.Desktop.Features.ReadingRoom.Components;
using RadiologyCenter.Desktop.Features.ReadingRoom.Models;

namespace RadiologyCenter.Desktop.Features.ReadingRoom.Pages;

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
    [Inject] private ApiClient Api { get; set; } = default!;

    private IReadOnlyList<EnumOptionDto> _severityOptions = Array.Empty<EnumOptionDto>();

    private readonly List<QueueExam> _queue = new();
    private readonly HashSet<string> _linking = new(StringComparer.OrdinalIgnoreCase);

    private string? _filter;
    private int _queuePage=1,_queueTotal,_pendingTotal,_imagedTotal;
    private bool _includeFinalized;
    private bool _loadingQueue;
    private string? _queueError;
    private bool _loaded;
    private readonly CancellationTokenSource _queuePolling = new();

    private QueueExam? _selected;
    private bool _loadingReport;
    private string? _reportError;
    private ReportDto? _report;


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

    private record CanonicalSection(string Type, int Position);

    private static readonly CanonicalSection[] CanonicalTypes =
    {
        new("ClinicalIndication", 1),
        new("Technique", 2),
        new("Findings", 3),
        new("Impression", 4),
        new("Recommendation", 5),
    };

    private string GetCanonicalLabel(string type) => type switch
    {
        "ClinicalIndication" => T.ReadingRoom.ClinicalIndication,
        "Technique" => T.ReadingRoom.Technique,
        "Findings" => T.ReadingRoom.Findings,
        "Impression" => T.ReadingRoom.Impression,
        "Recommendation" => T.ReadingRoom.Recommendation,
        _ => type,
    };

    private bool CanEdit => _report is { StatusKey: "Draft" };

    private QueueExam? Selected => _selected;


    private string? SelectedStudyUrl
    {
        get
        {
            if (PacsService.Instance?.IsReady!=true || _selected is not { StudyInstanceUid.Length: > 0 } selected)
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
            _ = LoadQueueAsync().ContinueWith(t =>
            {
                if (t.IsFaulted)
                    Snackbar.Add(T.ReadingRoom.Unreachable, Severity.Error);
            }, TaskContinuationOptions.OnlyOnFaulted);
            _ = PollQueueAsync();
        }
    }

    private async Task PollQueueAsync()
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(15));
        try {
            while (await timer.WaitForNextTickAsync(_queuePolling.Token))
                await InvokeAsync(async () => { if (!_loadingQueue) { await LoadQueueAsync(); StateHasChanged(); } });
        } catch (OperationCanceledException) when (_queuePolling.IsCancellationRequested) { }
    }
    private async Task ConfigureImagesAsync()
    {
        var dialog=await DialogService.ShowAsync<Shared.Components.PacsAccountDialog>(string.Empty);
        await dialog.Result;
        StateHasChanged();
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
        _queuePolling.Cancel();
        _jsRef?.Dispose();
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
        public QueueExam() { }
        public string Id { get; init; } = string.Empty;
        public string PatientId { get; init; } = string.Empty;
        public string PatientName { get; init; } = string.Empty;
        public string PatientCode { get; init; } = string.Empty;
        public string ExamName { get; init; } = string.Empty;
        public string ExaminationTypeId { get; init; } = string.Empty;
        public string StatusKey { get; init; } = "Completed";
        public DateTime? ScheduledAt { get; init; }
        public DateTime? CompletedAt { get; init; }
        public string Priority { get; init; } = string.Empty;
        public string PriorityKey { get; init; } = string.Empty;
        public string Indication { get; init; } = string.Empty;
        public string? AssignedRadiologistId { get; set; }
        public string? AssignedTechnicianId { get; set; }
        public string ReportStatus { get; set; } = "New";
        public string ReportStatusKey { get; set; } = "New";
        public string? AccessionNumber { get; init; }
        public string? StudyInstanceUid { get; init; }

        public string DateLabel => (ScheduledAt??CompletedAt) is {} date?ClinicTime.ToLocal(date).ToString("yyyy-MM-dd"):string.Empty;
    }

}
