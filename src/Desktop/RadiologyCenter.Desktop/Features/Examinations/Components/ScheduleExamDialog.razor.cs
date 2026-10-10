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

    private IReadOnlyList<PatientDto> _patients = Array.Empty<PatientDto>();
    private IReadOnlyList<ExaminationTypeDto> _examTypes = Array.Empty<ExaminationTypeDto>();

    private PatientDto? _selectedPatient;
    private ExaminationTypeDto? _selectedExamType;
    private string _priority = "Routine";
    private string? _clinicalIndication;
    private string? _notes;
    private string? _equipmentName;

    protected override async Task OnInitializedAsync()
    {
        _editContext = new EditContext(_model);

        _model.ScheduledDate = ScheduledDate ?? ClinicTime.Today;
        _model.ScheduledTime = ScheduledTime ?? TimeSpan.FromHours(9);
        _equipmentName = EquipmentName;

        _loading = false;
        await Task.CompletedTask;
    }

    private async Task<IEnumerable<PatientDto>> SearchPatients(string value, CancellationToken ct)
    {
        var page = await PatientService.GetPagedAsync(value, "LastName", false, 1, 50, isActive: true, ct: ct);
        return page.Items;
    }

    private async Task<IEnumerable<ExaminationTypeDto>> SearchExamTypes(string value, CancellationToken ct)
    {
        var page = await ExaminationService.GetTypesPagedAsync(value, "Name", false, 1, 50, modality: Modality, isActive: true, ct: ct);
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

    private async Task SubmitAsync()
    {
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

        await SafeExecute.RunAsync(async () =>
        {
            var scheduledAt = DateTime.SpecifyKind(_model.ScheduledDate!.Value.Date + _model.ScheduledTime!.Value, DateTimeKind.Unspecified);

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
        },
        Snackbar,
        () => T.ScheduleExam.CreateFailed,
        busy => _saving = busy);
    }

    private void Cancel() => MudDialog.Cancel();

}

