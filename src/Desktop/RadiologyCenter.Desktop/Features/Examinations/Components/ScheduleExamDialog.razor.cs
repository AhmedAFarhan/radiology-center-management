using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using RadiologyCenter.Desktop.Features.Examinations.Models;
using RadiologyCenter.Desktop.Features.Patients.Components;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Security;
using RadiologyCenter.Desktop.Security.Permissions;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Features.Examinations.Components;

public partial class ScheduleExamDialog : ComponentBase, IDisposable
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
    [Inject] private EnumOptionsService EnumOptionsService { get; set; } = default!;
    [Inject] private PermissionService Permissions { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;
    [Inject] private AppLocalizer T { get; set; } = default!;

    private readonly ScheduleExamFormModel _model = new();
    private readonly CancellationTokenSource _lifetime = new();
    private EditContext _editContext = default!;
    private bool _loading = true;
    private bool _saving, _disposed, _submitAttempted, _focusValidation, _openingRegistration;
    private IReadOnlyList<EquipmentDto> _equipment = [];
    private string? _formError;
    private ElementReference _validationSummary;
    private bool _quickRegistrationOpen, _patientSaving, _patientSearchFailed, _patientNotFound, _focusExamination, _focusPatient, _changingPatient;
    private string? _patientSearchText, _lastPatientQuery, _registrationSeed, _registeredPatientId;
    private QuickPatientRegistration? _registrationForm;
    private PatientInput? _patientDraft;
    private int _patientSearchVersion;
    private IReadOnlyList<EnumOptionDto> _genderOptions = [];
    private MudAutocomplete<PatientDto>? _patientLookup;
    private MudAutocomplete<ExaminationTypeDto>? _examinationLookup;
    private DialogOptions? _optionsBeforePatientSave;

    private bool IsBusy => _saving || _patientSaving || _openingRegistration;
    private bool CanRegisterPatient => Permissions.HasPermission(PatientsPermissions.Create);
    private string L(string en, string ar) => T.IsRTL ? ar : en;
    private static string ModalityKey(EquipmentDto equipment) => string.IsNullOrEmpty(equipment.ModalityKey) ? equipment.Modality : equipment.ModalityKey;
    private static string ModalityKey(ExaminationTypeDto type) => string.IsNullOrEmpty(type.ModalityKey) ? type.Modality : type.ModalityKey;
    private int DurationMinutes => _model.ExaminationType is { StandardDurationMinutes: > 0 } type ? type.StandardDurationMinutes : 30;
    private DateTime? StartTime => _model.ScheduledDate.HasValue && _model.ScheduledTime.HasValue
        ? _model.ScheduledDate.Value.Date + _model.ScheduledTime.Value : null;

    protected override async Task OnInitializedAsync()
    {
        _editContext = new EditContext(_model);
        _editContext.OnFieldChanged += OnFieldChanged;
        _model.ScheduledDate = ScheduledDate ?? ClinicTime.Today;
        _model.ScheduledTime = ScheduledTime ?? TimeSpan.FromHours(9);
        _model.EquipmentId = EquipmentId;
        try
        {
            var equipment = new List<EquipmentDto>();
            for (var pageNumber = 1; ; pageNumber++)
            {
                var page = await ResourceService.GetEquipmentPagedAsync(null, "Name", false, pageNumber, 100, status: "Operational", ct: _lifetime.Token);
                equipment.AddRange(page.Items.Where(e => e.IsActive && (string.IsNullOrEmpty(Modality) || ModalityKey(e) == Modality)));
                if (pageNumber * page.PageSize >= page.TotalCount || page.Items.Count == 0) break;
            }
            if (_disposed) return;
            _equipment = equipment;
            if (!_equipment.Any(e => e.Id == _model.EquipmentId)) _model.EquipmentId = null;
            if (_equipment.Count == 0)
                _formError = L("No operational equipment is available for this booking.", "لا توجد أجهزة جاهزة للعمل ومتاحة لهذا الحجز.");
        }
        catch (OperationCanceledException) when (_lifetime.IsCancellationRequested) { }
        catch { if (!_disposed) _formError = L("Equipment could not be loaded. Close this window and try again.", "تعذر تحميل الأجهزة. أغلق النافذة وحاول مرة أخرى."); }
        finally { _loading = false; }
    }

    private void OnFieldChanged(object? sender, FieldChangedEventArgs e)
    {
        if (_equipment.Count > 0 && e.FieldIdentifier.FieldName is nameof(ScheduleExamFormModel.EquipmentId)
            or nameof(ScheduleExamFormModel.ExaminationType) or nameof(ScheduleExamFormModel.ScheduledDate) or nameof(ScheduleExamFormModel.ScheduledTime))
            _formError = null;
    }
    private async Task<IEnumerable<PatientDto>> SearchPatientsAsync(string? value, CancellationToken ct)
    {
        var version = ++_patientSearchVersion;
        _patientNotFound = false;
        _patientSearchFailed = false;
        if (string.IsNullOrWhiteSpace(value))
            return Array.Empty<PatientDto>();
        _lastPatientQuery = value;
        try
        {
            var page = await PatientService.GetPagedAsync(value, "LastName", false, 1, 20, gender: null, isActive: true, ct);
            ct.ThrowIfCancellationRequested();
            if (!_disposed && version == _patientSearchVersion) _patientNotFound = page.Items.Count == 0;
            return page.Items;
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
        catch (Exception ex)
        {
            if (!_disposed && version == _patientSearchVersion)
            {
                _patientSearchFailed = true;
                Snackbar.Add(ex is ApiException api ? SafeExecute.FormatError(api) : T.VisitDialog.SearchPatientsError, Severity.Error);
            }
            return Array.Empty<PatientDto>();
        }
    }

    private void PatientSearchTextChanged(string? text)
    {
        _patientSearchText = text;
        if (!string.Equals(text, _lastPatientQuery, StringComparison.Ordinal))
        {
            ++_patientSearchVersion;
            _patientNotFound = _patientSearchFailed = false;
        }
    }

    private void ClearPatientSearch()
    {
        ++_patientSearchVersion;
        _patientSearchText = _lastPatientQuery = null;
        _patientSearchFailed = false;
        _patientNotFound = false;
    }

    private void PatientSelected(PatientDto? patient)
    {
        _model.Patient = patient;
        if (patient is not null) _changingPatient = false;
        _editContext.NotifyFieldChanged(_editContext.Field(nameof(ScheduleExamFormModel.Patient)));
    }

    private void ChangePatient()
    {
        _model.Patient = null;
        _editContext.NotifyFieldChanged(_editContext.Field(nameof(ScheduleExamFormModel.Patient)));
        _changingPatient = true;
        _focusPatient = true;
        ClearPatientSearch();
    }

    private async Task OpenQuickRegistrationAsync()
    {
        if (IsBusy || !CanRegisterPatient || !_patientNotFound || _patientSearchFailed) return;
        _openingRegistration = true;
        try
        {
            // Capture the query before autocomplete closes and coerces its display text.
            _registrationSeed = _model.Patient is not null ? null
                : !string.IsNullOrWhiteSpace(_patientSearchText) ? _patientSearchText : _lastPatientQuery;
            if (_patientLookup is not null) await _patientLookup.CloseMenuAsync();
            if (_genderOptions.Count == 0)
            {
                var options = await SafeExecute.RunAsync(() => EnumOptionsService.GetOptionsAsync("Gender", _lifetime.Token), Snackbar, () => T.VisitDialog.Unreachable);
                if (options is null || options.Count == 0) return;
                _genderOptions = options;
            }
            if (!_disposed) _quickRegistrationOpen = true;
        }
        finally { _openingRegistration = false; }
    }

    private void CloseQuickRegistration()
    {
        if (IsBusy) return;
        _patientDraft = _registrationForm?.GetDraft();
        _quickRegistrationOpen = false;
        _focusPatient = _model.Patient is null || _changingPatient;
    }

    private Task RegisterPatientAsync() => _registrationForm?.RegisterAsync() ?? Task.CompletedTask;

    private Task<PatientDto?> CreateQuickPatientAsync(PatientInput input)
        => SafeExecute.RunAsync(() => PatientService.CreateAsync(input), Snackbar, () => T.PatientDialog.Unreachable);

    private async Task PatientSavingChangedAsync(bool saving)
    {
        _patientSaving = saving;
        if (saving)
        {
            _optionsBeforePatientSave = MudDialog.Options;
            await MudDialog.SetOptionsAsync(MudDialog.Options with { BackdropClick = false, CloseOnEscapeKey = false, CloseButton = false });
        }
        else if (_optionsBeforePatientSave is { } original)
        {
            _optionsBeforePatientSave = null;
            if (!_disposed) await MudDialog.SetOptionsAsync(original);
        }
    }

    private Task UseRegisteredPatientAsync(PatientDto patient)
    {
        _model.Patient = patient;
        _patientDraft = null;
        _changingPatient = false;
        _registeredPatientId = patient.Id;
        _editContext.NotifyFieldChanged(_editContext.Field(nameof(ScheduleExamFormModel.Patient)));
        _quickRegistrationOpen = false;
        _focusExamination = true;
        Snackbar.Add(T["visitDialog.patientRegistered"], Severity.Success);
        return Task.CompletedTask;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_focusValidation && !_quickRegistrationOpen && (_formError is not null || _editContext.GetValidationMessages().Any()))
        {
            _focusValidation = false;
            await _validationSummary.FocusAsync();
        }
        if (_focusPatient && !_quickRegistrationOpen && _patientLookup is not null)
        {
            _focusPatient = false;
            await _patientLookup.FocusAsync();
        }
        if (_focusExamination && _examinationLookup is not null)
        {
            _focusExamination = false;
            await _examinationLookup.FocusAsync();
        }
    }

    private async Task<IEnumerable<ExaminationTypeDto>> SearchExamTypes(string? value, CancellationToken ct)
    {
        var selected = _equipment.FirstOrDefault(e => e.Id == _model.EquipmentId);
        var modality = selected is not null ? ModalityKey(selected) : Modality;
        try
        {
            var page = await ExaminationService.GetTypesPagedAsync(value, "Name", false, 1, 50, modality: modality, isActive: true, ct: ct);
            ct.ThrowIfCancellationRequested();
            return page.Items;
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
        catch (Exception ex)
        {
            if (!_disposed) Snackbar.Add(ex is ApiException api ? SafeExecute.FormatError(api) : T.VisitDialog.SearchTypesError, Severity.Error);
            return Array.Empty<ExaminationTypeDto>();
        }
    }

    private void OnExamTypeChanged(ExaminationTypeDto? examType)
    {
        _model.ExaminationType = examType;
        _editContext.NotifyFieldChanged(_editContext.Field(nameof(ScheduleExamFormModel.ExaminationType)));
    }

    private void OnEquipmentChanged(string? id)
    {
        _model.EquipmentId = id;
        _editContext.NotifyFieldChanged(_editContext.Field(nameof(ScheduleExamFormModel.EquipmentId)));
        if (_model.ExaminationType is not null && _equipment.FirstOrDefault(e => e.Id == id) is { } equipment
            && ModalityKey(equipment) != ModalityKey(_model.ExaminationType))
            OnExamTypeChanged(null);
    }

    private void ShowFormError(string error)
    {
        _formError = error;
        _focusValidation = true;
    }

    private async Task SubmitAsync()
    {
        if (IsBusy || _loading || _quickRegistrationOpen || _equipment.Count == 0) return;
        _formError = null;
        _submitAttempted = true;
        if (!_editContext.Validate())
        {
            _focusValidation = true;
            return;
        }

        var patient = _model.Patient!;
        var examType = _model.ExaminationType!;
        if (!_equipment.Any(e => e.Id == _model.EquipmentId && ModalityKey(e) == ModalityKey(examType)))
        {
            ShowFormError(L("Choose operational equipment for this examination.", "اختر جهازاً جاهزاً للعمل ومناسباً لهذا الفحص."));
            return;
        }
        if (StartTime is null || StartTime <= ClinicTime.Now)
        {
            ShowFormError(L("Choose a future appointment time.", "اختر وقتاً للموعد في المستقبل."));
            return;
        }

        _saving = true;
        var originalOptions = MudDialog.Options;
        bool created = false;
        try
        {
            await MudDialog.SetOptionsAsync(originalOptions with { BackdropClick = false, CloseOnEscapeKey = false, CloseButton = false });
            created = await SafeExecute.RunAsync(async () =>
            {
                var scheduledAt = DateTime.SpecifyKind(StartTime.Value, DateTimeKind.Unspecified);
                var end = scheduledAt.AddMinutes(DurationMinutes);
                var conflicts = await ExaminationService.GetCalendarSlotsAsync(ClinicTime.ToUtc(scheduledAt), ClinicTime.ToUtc(end), Guid.Parse(_model.EquipmentId!));
                if (conflicts.Any(e => e.Status != "Cancelled" && ClinicTime.ToLocal(e.ScheduledAt) < end && ClinicTime.ToLocal(e.ScheduledEnd ?? e.ScheduledAt.AddMinutes(30)) > scheduledAt))
                {
                    ShowFormError(L("This equipment is already booked during part of the examination. Choose another time or equipment.", "الجهاز محجوز خلال جزء من مدة الفحص. اختر وقتاً أو جهازاً آخر."));
                    return false;
                }

                await ExaminationService.BookAsync(new BookExamInput
                {
                    PatientId = patient.Id,
                    ExaminationTypeId = examType.Id,
                    EquipmentId = _model.EquipmentId,
                    ClinicalIndication = _model.ClinicalIndication?.Trim(),
                    Priority = _model.Priority,
                    Notes = _model.Notes?.Trim(),
                    ScheduledAt = scheduledAt.ToString("O")
                });
                return true;
            }, Snackbar, () => T.ScheduleExam.CreateFailed);
        }
        finally
        {
            _saving = false;
            if (!_disposed) await MudDialog.SetOptionsAsync(originalOptions);
        }
        if (created && !_disposed)
        {
            Snackbar.Add(T.ScheduleExam.Created, Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
    }

    private void Cancel() { if (!IsBusy) MudDialog.Cancel(); }

    public void Dispose()
    {
        _disposed = true;
        ++_patientSearchVersion;
        _editContext.OnFieldChanged -= OnFieldChanged;
        _lifetime.Cancel();
        _lifetime.Dispose();
    }
}
