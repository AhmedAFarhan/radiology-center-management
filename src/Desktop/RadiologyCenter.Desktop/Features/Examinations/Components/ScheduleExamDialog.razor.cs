using Microsoft.AspNetCore.Components;
using MudBlazor;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Features.Examinations.Components;

public partial class ScheduleExamDialog : ComponentBase
{
    [Parameter] public string? EquipmentId { get; set; }
    [Parameter] public string? EquipmentName { get; set; }
    [Parameter] public string? Modality { get; set; }
    [Parameter] public DateTime? ScheduledDate { get; set; }
    [Parameter] public TimeSpan? ScheduledTime { get; set; }

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    [Inject] private ExaminationService ExaminationService { get; set; } = default!;
    [Inject] private PatientService PatientService { get; set; } = default!;
    [Inject] private ResourceService ResourceService { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;
    [Inject] private AppLocalizer T { get; set; } = default!;

    private bool _loading = true;
    private bool _saving;

    private IReadOnlyList<PatientDto> _patients = Array.Empty<PatientDto>();
    private IReadOnlyList<ExaminationTypeDto> _examTypes = Array.Empty<ExaminationTypeDto>();

    private PatientDto? _selectedPatient;
    private ExaminationTypeDto? _selectedExamType;
    private DateTime? _scheduledDate;
    private TimeSpan? _scheduledTime;
    private string _priority = "Routine";
    private string? _clinicalIndication;
    private string? _notes;
    private string? _equipmentName;

    protected override async Task OnInitializedAsync()
    {
        _scheduledDate = ScheduledDate ?? DateTime.Today;
        _scheduledTime = ScheduledTime ?? TimeSpan.FromHours(9);
        _equipmentName = EquipmentName;

        try
        {
            var patientTask = PatientService.GetPagedAsync(null, null, false, 1, 500);
            var typeTask = ExaminationService.GetTypesPagedAsync(null, null, false, 1, 500);

            await Task.WhenAll(patientTask, typeTask);

            _patients = (await patientTask).Items.Where(p => p.IsActive).ToList();
            _examTypes = (await typeTask).Items.Where(t => t.IsActive).ToList();

            if (!string.IsNullOrEmpty(Modality))
                _examTypes = _examTypes
                    .Where(t => string.Equals(t.Modality, Modality, StringComparison.OrdinalIgnoreCase))
                    .ToList();
        }
        catch
        {
            Snackbar.Add(T.ScheduleExam.LoadError, Severity.Warning);
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task<IEnumerable<PatientDto>> SearchPatients(string value, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(value))
            return _patients.Take(50);

        await Task.Delay(0, ct);
        return _patients
            .Where(p => p.FullName.Contains(value, StringComparison.OrdinalIgnoreCase)
                     || p.PatientCode.Contains(value, StringComparison.OrdinalIgnoreCase)
                     || p.PhoneNumber.Contains(value))
            .Take(50);
    }

    private async Task<IEnumerable<ExaminationTypeDto>> SearchExamTypes(string value, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(value))
            return _examTypes.Take(50);

        await Task.Delay(0, ct);
        return _examTypes
            .Where(t => t.Name.Contains(value, StringComparison.OrdinalIgnoreCase)
                     || t.Code.Contains(value, StringComparison.OrdinalIgnoreCase))
            .Take(50);
    }

    private void OnPatientChanged(PatientDto? patient)
    {
        _selectedPatient = patient;
    }

    private void OnExamTypeChanged(ExaminationTypeDto? examType)
    {
        _selectedExamType = examType;
    }

    private async Task SubmitAsync()
    {
        if (_selectedPatient is null)
        {
            Snackbar.Add(T.ScheduleExam.PatientRequired, Severity.Warning);
            return;
        }

        if (_selectedExamType is null)
        {
            Snackbar.Add(T.ScheduleExam.ExamTypeRequired, Severity.Warning);
            return;
        }

        if (_scheduledDate is null || _scheduledTime is null)
        {
            Snackbar.Add(T.ScheduleExam.DateTimeRequired, Severity.Warning);
            return;
        }

        _saving = true;
        StateHasChanged();

        try
        {
            var scheduledAt = _scheduledDate.Value.Date + _scheduledTime.Value;

            var input = new BookExamInput
            {
                PatientId = _selectedPatient.Id,
                ExaminationTypeId = _selectedExamType.Id,
                EquipmentId = EquipmentId,
                ClinicalIndication = _clinicalIndication?.Trim(),
                Priority = _priority,
                Notes = _notes?.Trim(),
                ScheduledAt = scheduledAt.ToString("O")
            };

            await ExaminationService.BookAsync(input);
            Snackbar.Add(T.ScheduleExam.Created, Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
        catch (ApiException apiEx)
        {
            Snackbar.Add($"{T.ScheduleExam.CreateFailed}: {apiEx.Message}", Severity.Error);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"{T.ScheduleExam.CreateFailed}: {ex.Message}", Severity.Error);
        }
        finally
        {
            _saving = false;
        }
    }

    private void Cancel() => MudDialog.Cancel();
}

