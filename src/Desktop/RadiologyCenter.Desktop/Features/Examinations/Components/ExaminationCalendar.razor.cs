using Microsoft.AspNetCore.Components;
using MudBlazor;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;
using static RadiologyCenter.Desktop.Services.ApiClient;

namespace RadiologyCenter.Desktop.Features.Examinations.Components;

public partial class ExaminationCalendar : ComponentBase
{
    [Inject] private ExaminationService ExaminationService { get; set; } = default!;
    [Inject] private ResourceService ResourceService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;
    [Inject] private IDialogService DialogService { get; set; } = default!;
    [Inject] private AppLocalizer T { get; set; } = default!;

    private DateTime _currentDate = DateTime.Today;
    private string _viewMode = "day";
    private string? _selectedModality;
    private string? _selectedEquipmentId;
    private bool _loading = true;

    private IReadOnlyList<EquipmentDto> _equipments = Array.Empty<EquipmentDto>();
    private List<CalendarSlotDto> _slots = new();
    private List<ResourceRow> _resources = new();
    private List<string> _timeHeaders = new();
    private List<DateTime> _weekDays = new();
    private double? _currentTimeLeft;

    private ElementReference _timeGridRef;

    private static readonly string[] Modalities = ["CT", "MRI", "XRay", "Ultrasound", "Mammography", "Fluoroscopy", "DEXA"];
    private IReadOnlyList<string> _modalities = Modalities;

    private const int SlotIntervalMinutes = 30;
    private const int WorkDayStartHour = 8;
    private const int WorkDayEndHour = 17;
    private const int SlotWidthPx = 100;
    private const int ResourceColWidthPx = 180;

    protected override async Task OnInitializedAsync()
    {
        await LoadEquipmentsAsync();
        await LoadDataAsync();
    }

    private async Task LoadEquipmentsAsync()
    {
        try
        {
            var page = await ResourceService.GetEquipmentPagedAsync(null, null, false, 1, 200);
            _equipments = page.Items.Where(e => e.IsActive).ToList();
        }
        catch
        {
            // non-critical
        }
    }

    private async Task LoadDataAsync()
    {
        _loading = true;
        StateHasChanged();

        try
        {
            var (start, end) = GetDayRange();
            BuildWeekDays(start);

            var startUtc = start.ToUniversalTime();
            var endUtc = end.ToUniversalTime();

            _slots = (await ExaminationService.GetCalendarSlotsAsync(
                startUtc, endUtc,
                string.IsNullOrEmpty(_selectedEquipmentId) ? null : Guid.Parse(_selectedEquipmentId),
                null,
                _selectedModality)).ToList();

            BuildTimeHeaders();
            BuildResourceRows(start);
            UpdateCurrentTimeIndicator(start);
        }
        catch (ApiException apiEx)
        {
            var detail = apiEx.StatusCode > 0 ? $" (HTTP {apiEx.StatusCode})" : "";
            Snackbar.Add($"{T.ExamCalendar.LoadError}{detail}: {apiEx.Message}", Severity.Error);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"{T.ExamCalendar.LoadError}: {ex.Message}", Severity.Error);
        }
        finally
        {
            _loading = false;
        }
    }

    private (DateTime DayStart, DateTime DayEnd) GetDayRange()
    {
        var date = _currentDate.Date;
        return (date.AddHours(WorkDayStartHour), date.AddHours(WorkDayEndHour));
    }

    private void BuildWeekDays(DateTime weekStart)
    {
        _weekDays.Clear();
        if (_viewMode != "week") return;

        var monday = weekStart.Date.AddDays(-(int)weekStart.Date.DayOfWeek + (int)DayOfWeek.Monday);
        for (var i = 0; i < 7; i++)
            _weekDays.Add(monday.AddDays(i));
    }

    private void BuildTimeHeaders()
    {
        _timeHeaders.Clear();
        var date = _currentDate.Date;
        var current = date.AddHours(WorkDayStartHour);
        var end = date.AddHours(WorkDayEndHour);

        while (current < end)
        {
            _timeHeaders.Add(current.ToString("HH:mm"));
            current = current.AddMinutes(SlotIntervalMinutes);
        }
    }

    private void BuildResourceRows(DateTime dayStart)
    {
        _resources.Clear();

        var resourceSlots = _selectedEquipmentId is not null
            ? _slots.Where(s => s.EquipmentId == _selectedEquipmentId).ToList()
            : _slots;

        var groupedByEquipment = resourceSlots
            .Where(s => s.EquipmentId is not null)
            .GroupBy(s => s.EquipmentId!)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var eq in _equipments)
        {
            if (_selectedEquipmentId is not null && eq.Id != _selectedEquipmentId)
                continue;

            if (!string.IsNullOrEmpty(_selectedModality)
                && !string.Equals(eq.Modality, _selectedModality, StringComparison.OrdinalIgnoreCase))
                continue;

            var row = new ResourceRow { Name = eq.Name, Type = eq.Modality, Slots = new List<SlotCell>() };

            var current = dayStart;
            while (current < dayStart.AddHours(WorkDayEndHour - WorkDayStartHour))
            {
                var slotEnd = current.AddMinutes(SlotIntervalMinutes);
                var exam = groupedByEquipment.TryGetValue(eq.Id, out var exams)
                    ? exams.FirstOrDefault(e => e.ScheduledAt < slotEnd && current < (e.ScheduledEnd ?? e.ScheduledAt.AddMinutes(30)))
                    : null;

                row.Slots.Add(new SlotCell
                {
                    StartTime = current,
                    EndTime = slotEnd,
                    IsAvailable = exam is null,
                    Exam = exam
                });

                current = slotEnd;
            }

            _resources.Add(row);
        }
    }

    private void UpdateCurrentTimeIndicator(DateTime dayStart)
    {
        _currentTimeLeft = null;
        var now = DateTime.Now;

        if (now.Date != _currentDate.Date)
            return;

        var workStart = dayStart;
        var workEnd = dayStart.AddHours(WorkDayEndHour - WorkDayStartHour);

        if (now < workStart || now > workEnd)
            return;

        var minutesSinceStart = (now - workStart).TotalMinutes;
        var slotIndex = minutesSinceStart / SlotIntervalMinutes;
        _currentTimeLeft = slotIndex * SlotWidthPx;
    }

    private string GetSlotStatusClass(SlotCell slot)
    {
        if (slot.IsAvailable) return string.Empty;

        return slot.Exam?.Status switch
        {
            "Scheduled" => "status-scheduled",
            "CheckedIn" => "status-checked-in",
            "InProgress" => "status-in-progress",
            _ => string.Empty
        };
    }

    private string GetModalityColor(string modality) => modality?.ToUpperInvariant() switch
    {
        "CT" => "#1976D2",
        "MRI" => "#7B1FA2",
        "XRAY" => "#388E3C",
        "ULTRASOUND" => "#F57C00",
        "MAMMOGRAPHY" => "#C2185B",
        "FLUOROSCOPY" => "#00796B",
        "DEXA" => "#5D4037",
        _ => "#757575"
    };

    private async Task OnSlotClick(SlotCell slot)
    {
        if (!slot.IsAvailable && slot.Exam is not null)
        {
            NavigationManager.NavigateTo($"/visits?highlight={slot.Exam.Id}");
            return;
        }

        var equipmentName = _equipments.FirstOrDefault(e => e.Id == _selectedEquipmentId)?.Name;

        var parameters = new DialogParameters
        {
            ["EquipmentId"] = _selectedEquipmentId,
            ["EquipmentName"] = equipmentName,
            ["Modality"] = _selectedModality,
            ["ScheduledDate"] = slot.StartTime.Date,
            ["ScheduledTime"] = slot.StartTime.TimeOfDay,
        };
        var dialog = await DialogService.ShowAsync<ScheduleExamDialog>(T.ExamCalendar.ScheduleExam, parameters);
        var result = await dialog.Result;
        if (result is { Data: true })
            await LoadDataAsync();
    }

    private async Task SelectDayAsync(DateTime date)
    {
        _currentDate = date;
        await LoadDataAsync();
    }

    private async Task PreviousDay()
    {
        _currentDate = _viewMode == "week" ? _currentDate.AddDays(-7) : _currentDate.AddDays(-1);
        await LoadDataAsync();
    }

    private async Task NextDay()
    {
        _currentDate = _viewMode == "week" ? _currentDate.AddDays(7) : _currentDate.AddDays(1);
        await LoadDataAsync();
    }

    private async Task GoToToday()
    {
        _currentDate = DateTime.Today;
        await LoadDataAsync();
    }

    private async Task OnViewModeChanged(string value)
    {
        _viewMode = value;
        await LoadDataAsync();
    }

    private async Task OnModalityChanged(string? value)
    {
        _selectedModality = value;
        await LoadDataAsync();
    }

    private async Task OnEquipmentChanged(string? value)
    {
        _selectedEquipmentId = value;
        await LoadDataAsync();
    }

    private sealed class ResourceRow
    {
        public string Name { get; init; } = string.Empty;
        public string Type { get; init; } = string.Empty;
        public List<SlotCell> Slots { get; init; } = new();
    }

    private sealed class SlotCell
    {
        public DateTime StartTime { get; init; }
        public DateTime EndTime { get; init; }
        public bool IsAvailable { get; init; }
        public CalendarSlotDto? Exam { get; init; }
    }
}

