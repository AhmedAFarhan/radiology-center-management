using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;

using RadiologyCenter.Desktop.Features.Examinations.Models;

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

    private readonly ScheduleExamFormModel _model = new();
    private EditContext _editContext = default!;
    private bool _loading = true;
    private bool _saving;

    private IReadOnlyList<EquipmentDto> _equipment = Array.Empty<EquipmentDto>();
    private string? _equipmentId;
    private string? _formError;

    private PatientDto? _selectedPatient;
    private ExaminationTypeDto? _selectedExamType;
    private string _priority = "Routine";
    private string? _clinicalIndication;
    private string? _notes;
    private string L(string en, string ar) => T.IsRTL ? ar : en;
    private static string ModalityKey(EquipmentDto equipment) => string.IsNullOrEmpty(equipment.ModalityKey) ? equipment.Modality : equipment.ModalityKey;
    private static string ModalityKey(ExaminationTypeDto type) => string.IsNullOrEmpty(type.ModalityKey) ? type.Modality : type.ModalityKey;
    private int DurationMinutes => _selectedExamType is { StandardDurationMinutes: > 0 } type ? type.StandardDurationMinutes : 30;
    private DateTime? StartTime => _model.ScheduledDate.HasValue && _model.ScheduledTime.HasValue
        ? _model.ScheduledDate.Value.Date + _model.ScheduledTime.Value : null;

    protected override async Task OnInitializedAsync()
    {
        _editContext = new EditContext(_model);

        _model.ScheduledDate = ScheduledDate ?? ClinicTime.Today;
        _model.ScheduledTime = ScheduledTime ?? TimeSpan.FromHours(9);
        _equipmentId = EquipmentId;
        try
        {
            var equipment = new List<EquipmentDto>();
            for (var pageNumber = 1; ; pageNumber++)
            {
                var page = await ResourceService.GetEquipmentPagedAsync(null, "Name", false, pageNumber, 100, status: "Operational");
                equipment.AddRange(page.Items.Where(e => e.IsActive && (string.IsNullOrEmpty(Modality) || ModalityKey(e) == Modality)));
                if (pageNumber * page.PageSize >= page.TotalCount || page.Items.Count == 0) break;
            }
            _equipment = equipment;
        }
        catch { _formError = L("Equipment could not be loaded. Close this window and try again.", "تعذر تحميل الأجهزة. أغلق النافذة وحاول مرة أخرى."); }
        finally { _loading = false; }
    }

    private async Task<IEnumerable<PatientDto>> SearchPatients(string value, CancellationToken ct)
    {
        var page = await PatientService.GetPagedAsync(value, "LastName", false, 1, 50, isActive: true, ct: ct);
        return page.Items;
    }

    private async Task<IEnumerable<ExaminationTypeDto>> SearchExamTypes(string value, CancellationToken ct)
    {
        var selected = _equipment.FirstOrDefault(e => e.Id == _equipmentId);
        var modality = selected is not null ? ModalityKey(selected) : Modality;
        var page = await ExaminationService.GetTypesPagedAsync(value, "Name", false, 1, 50, modality: modality, isActive: true, ct: ct);
        return page.Items;
    }

    private void OnPatientChanged(PatientDto? patient)
    {
        _selectedPatient = patient;
    }

    private void OnExamTypeChanged(ExaminationTypeDto? examType)
    {
        _selectedExamType = examType;
    }

    private void OnEquipmentChanged(string? id)
    {
        _equipmentId = id;
        _formError = null;
        if (_selectedExamType is not null && _equipment.FirstOrDefault(e => e.Id == id) is { } equipment && ModalityKey(equipment) != ModalityKey(_selectedExamType))
            _selectedExamType = null;
    }

    private async Task SubmitAsync()
    {
        if (_saving) return;
        _formError = null;
        if (!_editContext.Validate())
            return;

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

        if (!_equipment.Any(e => e.Id == _equipmentId && ModalityKey(e) == ModalityKey(_selectedExamType)))
        {
            _formError = L("Choose operational equipment for this examination.", "اختر جهازاً جاهزاً للعمل ومناسباً لهذا الفحص.");
            return;
        }
        if (StartTime is null || StartTime <= ClinicTime.Now)
        {
            _formError = L("Choose a future appointment time.", "اختر وقتاً للموعد في المستقبل.");
            return;
        }

        await SafeExecute.RunAsync(async () =>
        {
            var scheduledAt = DateTime.SpecifyKind(_model.ScheduledDate!.Value.Date + _model.ScheduledTime!.Value, DateTimeKind.Unspecified);

            var end = scheduledAt.AddMinutes(DurationMinutes);
            var conflicts = await ExaminationService.GetCalendarSlotsAsync(ClinicTime.ToUtc(scheduledAt), ClinicTime.ToUtc(end), Guid.Parse(_equipmentId!));
            if (conflicts.Any(e => e.Status != "Cancelled" && ClinicTime.ToLocal(e.ScheduledAt) < end && ClinicTime.ToLocal(e.ScheduledEnd ?? e.ScheduledAt.AddMinutes(30)) > scheduledAt))
            {
                _formError = L("This equipment is already booked during part of the examination. Choose another time or equipment.", "الجهاز محجوز خلال جزء من مدة الفحص. اختر وقتاً أو جهازاً آخر.");
                return;
            }

            var input = new BookExamInput
            {
                PatientId = _selectedPatient.Id,
                ExaminationTypeId = _selectedExamType.Id,
                EquipmentId = _equipmentId,
                ClinicalIndication = _clinicalIndication?.Trim(),
                Priority = _priority,
                Notes = _notes?.Trim(),
                ScheduledAt = scheduledAt.ToString("O")
            };

            await ExaminationService.BookAsync(input);
            Snackbar.Add(T.ScheduleExam.Created, Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        },
        Snackbar,
        () => T.ScheduleExam.CreateFailed,
        busy => _saving = busy);
    }

    private void Cancel() => MudDialog.Cancel();

}

