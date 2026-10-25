using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using RadiologyCenter.Desktop.Features.Examinations.Models;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Security;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Features.Examinations.Components;

public partial class ExaminationCalendar : ComponentBase, IDisposable
{
    [Inject] private ExaminationService ExaminationService { get; set; } = default!;
    [Inject] private ResourceService ResourceService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IDialogService DialogService { get; set; } = default!;
    [Inject] private AppLocalizer T { get; set; } = default!;
    [Inject] private PermissionService Permissions { get; set; } = default!;

    private readonly CancellationTokenSource _lifetime = new();
    private DateTime _currentDate = ClinicTime.Today;
    private DateTime _now = ClinicTime.Now;
    private string _viewMode = "day";
    private string? _selectedModality, _selectedEquipmentId, _error;
    private bool _loading = true, _disposed, _detailsOpen;
    private int _loadVersion;
    private CalendarSlotDto? _selectedExam;
    private IReadOnlyList<EquipmentDto> _equipments = [];
    private List<CalendarSlotDto> _slots = [];

    private string L(string en, string ar) => T.IsRTL ? ar : en;
    private CultureInfo Culture => CultureInfo.GetCultureInfo(T.IsRTL ? "ar-EG" : "en-GB");
    private bool CanBook => Permissions.HasPermission("examinations.create");
    private DateTime WeekStart => _currentDate.Date.AddDays(-(((int)_currentDate.DayOfWeek + 6) % 7));
    private IEnumerable<DateTime> WeekDays => Enumerable.Range(0, 7).Select(i => WeekStart.AddDays(i));
    private IEnumerable<EquipmentDto> VisibleEquipment => _equipments.Where(e =>
        (string.IsNullOrEmpty(_selectedEquipmentId) || e.Id == _selectedEquipmentId) &&
        (string.IsNullOrEmpty(_selectedModality) || ModalityKey(e) == _selectedModality));
    private IEnumerable<CalendarSlotDto> OnDay(DateTime day) => _slots.Where(e => e.ScheduledAt < day.Date.AddDays(1) && End(e) > day.Date);
    private IEnumerable<CalendarSlotDto> DayAppointments => OnDay(_currentDate);
    private IEnumerable<CalendarSlotDto> Unassigned => DayAppointments.Where(e => !VisibleEquipment.Any(eq => eq.Id == e.EquipmentId));
    private int StartHour => Math.Min(8, DayAppointments.Select(e => e.ScheduledAt.Date < _currentDate.Date ? 0 : e.ScheduledAt.Hour).DefaultIfEmpty(8).Min());
    private int EndHour => Math.Max(17, DayAppointments.Select(e => End(e).Date > _currentDate.Date ? 24 : (int)Math.Ceiling(End(e).TimeOfDay.TotalHours)).DefaultIfEmpty(17).Max());
    private DateTime GridStart => _currentDate.Date.AddHours(StartHour);
    private DateTime GridEnd => _currentDate.Date.AddHours(EndHour);
    private int TrackWidth => (EndHour - StartHour) * 120;
    private IEnumerable<DateTime> Ticks => Enumerable.Range(0, (EndHour - StartHour) * 2).Select(i => GridStart.AddMinutes(i * 30));
    private string DateTitle => _viewMode == "week"
        ? $"{WeekStart.ToString("d MMM", Culture)} – {WeekStart.AddDays(6).ToString("d MMM yyyy", Culture)}"
        : _currentDate.ToString("dddd، d MMMM yyyy", Culture);
    private static DateTime End(CalendarSlotDto exam) => exam.ScheduledEnd ?? exam.ScheduledAt.AddMinutes(30);
    private static string Px(double value) => value.ToString("0.##", CultureInfo.InvariantCulture) + "px";
    private IReadOnlyList<TimelinePlacement> Placements(string equipmentId) => CalendarTimeline.Arrange(
        DayAppointments.Where(e => e.EquipmentId == equipmentId).Select(e => new TimelineAppointment(e.Id, e.ScheduledAt, End(e))), GridStart, GridEnd);
    private string Status(CalendarSlotDto e) => e.Status switch
    {
        "Scheduled" => T.ExamCalendar.Scheduled, "CheckedIn" => T.ExamCalendar.CheckedIn,
        "InProgress" => T.ExamCalendar.InProgress, "Completed" => L("Completed", "مكتمل"),
        "Cancelled" => L("Cancelled", "ملغي"), _ => e.Status
    };
    private static string StatusClass(CalendarSlotDto e) => e.Status switch
    {
        "CheckedIn" => "checked-in", "InProgress" => "in-progress", "Completed" => "completed", _ => "scheduled"
    };
    private string Priority(CalendarSlotDto e) => e.Priority == "Stat" ? T.ScheduleExam.Emergency : T.ScheduleExam.Urgent;
    private static string ModalityKey(EquipmentDto equipment) => string.IsNullOrEmpty(equipment.ModalityKey) ? equipment.Modality : equipment.ModalityKey;
    private string EquipmentName(CalendarSlotDto e) => e.EquipmentName ?? _equipments.FirstOrDefault(eq => eq.Id == e.EquipmentId)?.Name
        ?? (string.IsNullOrEmpty(e.EquipmentId) ? L("No equipment assigned", "لم يتم تحديد جهاز") : L("Assigned equipment is not operational", "الجهاز المحدد غير متاح للعمل"));
    private string Description(CalendarSlotDto e) => $"{e.PatientName} · {e.ExaminationTypeName} · {e.ScheduledAt:HH:mm}–{End(e):HH:mm} · {Status(e)}";

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
        if (!_disposed) _ = UpdateClockAsync(_lifetime.Token);
    }

    private async Task LoadDataAsync()
    {
        var version = ++_loadVersion;
        var start = _viewMode == "week" ? WeekStart : _currentDate.Date;
        var end = start.AddDays(_viewMode == "week" ? 7 : 1);
        var equipmentId = Guid.TryParse(_selectedEquipmentId, out var id) ? id : (Guid?)null;
        var modality = _selectedModality;
        _loading = true;
        _error = null;
        _detailsOpen = false;
        try
        {
            var equipment = new List<EquipmentDto>();
            for (var pageNumber = 1; ; pageNumber++)
            {
                var page = await ResourceService.GetEquipmentPagedAsync(null, "Name", false, pageNumber, 100, status: "Operational", ct: _lifetime.Token);
                equipment.AddRange(page.Items.Where(e => e.IsActive));
                if (pageNumber * page.PageSize >= page.TotalCount || page.Items.Count == 0) break;
            }
            var slots = await ExaminationService.GetCalendarSlotsAsync(ClinicTime.ToUtc(start), ClinicTime.ToUtc(end), equipmentId, null, modality, _lifetime.Token);
            if (_disposed || version != _loadVersion) return;
            _equipments = equipment;
            _slots = slots.Select(e => e with { ScheduledAt = ClinicTime.ToLocal(e.ScheduledAt), ScheduledEnd = e.ScheduledEnd.HasValue ? ClinicTime.ToLocal(e.ScheduledEnd.Value) : null })
                .OrderBy(e => e.ScheduledAt).ToList();
            _now = ClinicTime.Now;
        }
        catch (OperationCanceledException) when (_lifetime.IsCancellationRequested) { }
        catch { if (!_disposed && version == _loadVersion) _error = T.ExamCalendar.LoadError; }
        finally { if (!_disposed && version == _loadVersion) _loading = false; }
    }

    private async Task UpdateClockAsync(CancellationToken token)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));
        try
        {
            while (await timer.WaitForNextTickAsync(token))
                await InvokeAsync(() => { _now = ClinicTime.Now; StateHasChanged(); });
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { }
    }

    private Task MoveAsync(int direction) { _currentDate = _currentDate.AddDays(direction * (_viewMode == "week" ? 7 : 1)); return LoadDataAsync(); }
    private Task TodayAsync() { _currentDate = ClinicTime.Today; return LoadDataAsync(); }
    private Task ChangeDateAsync(DateTime? date) { if (!date.HasValue) return Task.CompletedTask; _currentDate = date.Value.Date; return LoadDataAsync(); }
    private Task ChangeViewAsync(string view) { _viewMode = view; return LoadDataAsync(); }
    private Task OpenDayAsync(DateTime day) { _currentDate = day; _viewMode = "day"; return LoadDataAsync(); }
    private Task ModalityChangedAsync(string? value) { _selectedModality = value; _selectedEquipmentId = null; return LoadDataAsync(); }
    private Task EquipmentChangedAsync(string? value) { _selectedEquipmentId = value; return LoadDataAsync(); }
    private Task ClearFiltersAsync() { _selectedModality = null; _selectedEquipmentId = null; return LoadDataAsync(); }
    private void OnDetailsKeyDown(KeyboardEventArgs args) { if (args.Key == "Escape") _detailsOpen = false; }
    private void ShowDetails(CalendarSlotDto exam) { _selectedExam = exam; _detailsOpen = true; }
    private void OpenVisit() { if (_selectedExam is not null) NavigationManager.NavigateTo($"/visits?highlight={_selectedExam.Id}"); }

    private async Task BookAsync(DateTime? start = null, EquipmentDto? equipment = null)
    {
        if (!CanBook || _loading) return;
        var date = start ?? (_currentDate.Date == ClinicTime.Today ? ClinicTime.Now.AddMinutes(30) : _currentDate.Date.AddHours(9));
        if (date <= ClinicTime.Now) return;
        var selectedId = equipment?.Id ?? _selectedEquipmentId;
        var parameters = new DialogParameters
        {
            [nameof(ScheduleExamDialog.EquipmentId)] = selectedId,
            [nameof(ScheduleExamDialog.EquipmentName)] = equipment?.Name ?? _equipments.FirstOrDefault(e => e.Id == selectedId)?.Name,
            [nameof(ScheduleExamDialog.Modality)] = equipment is not null ? ModalityKey(equipment) : _selectedModality,
            [nameof(ScheduleExamDialog.ScheduledDate)] = date.Date,
            [nameof(ScheduleExamDialog.ScheduledTime)] = new TimeSpan(date.Hour, date.Minute, 0)
        };
        var dialog = await DialogService.ShowAsync<ScheduleExamDialog>(T.ExamCalendar.ScheduleExam, parameters,
            new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, BackdropClick = false });
        if (await dialog.Result is { Canceled: false, Data: true } && !_disposed) await LoadDataAsync();
    }

    public void Dispose() { _disposed = true; ++_loadVersion; _lifetime.Cancel(); _lifetime.Dispose(); }
}
