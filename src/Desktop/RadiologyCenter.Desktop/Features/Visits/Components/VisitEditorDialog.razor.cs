using System.Net.Http;
using System.Net.Http.Json;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using MudBlazor;
using RadiologyCenter.Desktop;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Features.Visits.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Features.Visits.Components;

public partial class VisitEditorDialog : ComponentBase, IDisposable
{
[Parameter] public ExaminationDto? Visit { get; set; }

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private IReadOnlyList<EnumOptionDto> _priorityOptions = Array.Empty<EnumOptionDto>();
    private IReadOnlyList<EnumOptionDto> _statusOptions = Array.Empty<EnumOptionDto>();
    private IReadOnlyList<StaffDto> _radiologists = Array.Empty<StaffDto>();
    private IReadOnlyList<StaffDto> _technicians = Array.Empty<StaffDto>();

    private readonly VisitFormModel _model = new();
    private EditContext _editContext = default!;
    private ValidationMessageStore? _scheduleMessages;
    private ReferralDoctorDto? _selectedReferralDoctor;
    private bool _busy;
    private bool _disposed;
    private bool _submitAttempted, _focusValidation;
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

    private bool IsBusy => _busy || _patientSaving;
    private bool CanRegisterPatient => Permissions.HasPermission(PatientsPermissions.Create);

    private bool IsEdit => Visit is not null;

    private static readonly TimeZoneInfo ClinicTimeZone = ResolveClinicTimeZone();

    private static TimeZoneInfo ResolveClinicTimeZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo");
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
        }
    }

    private static DateTime ToClinicLocal(DateTime utc) =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), ClinicTimeZone);

    private void OnEditContextFieldChanged(object? sender, FieldChangedEventArgs e)
    {
        if (_submitAttempted && e.FieldIdentifier.FieldName is nameof(VisitFormModel.ScheduledDate) or nameof(VisitFormModel.ScheduledTime))
            ValidateSchedule();
    }

    private void OnStatusChanged()
    {
        if (_submitAttempted) ValidateSchedule();
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        if (_editContext is not null)
            _editContext.OnFieldChanged -= OnEditContextFieldChanged;
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
            var page = await PatientService.GetPagedAsync(value, "LastName", false, 1, 20, gender: null, isActive: null, ct);
            ct.ThrowIfCancellationRequested();
            if (version == _patientSearchVersion) _patientNotFound = page.Items.Count == 0;
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

    private void PatientSearchTextChanged(string? text) => _patientSearchText = text;

    private void ClearPatientSearch()
    {
        _patientSearchText = _lastPatientQuery = null;
        _patientSearchFailed = false;
        _patientNotFound = false;
    }

    private void PatientSelected(PatientDto? patient)
    {
        _model.Patient = patient;
        if (patient is not null) _changingPatient = false;
        _editContext.NotifyFieldChanged(_editContext.Field(nameof(VisitFormModel.Patient)));
    }

    private void ChangePatient()
    {
        _model.Patient = null;
        _editContext.NotifyFieldChanged(_editContext.Field(nameof(VisitFormModel.Patient)));
        _changingPatient = true;
        _focusPatient = true;
        ClearPatientSearch();
    }

    private async Task OpenQuickRegistrationAsync()
    {
        if (IsBusy || !CanRegisterPatient || !_patientNotFound || _patientSearchFailed) return;
        // Capture the query before autocomplete closes and coerces its display text.
        _registrationSeed = _model.Patient is not null ? null
            : !string.IsNullOrWhiteSpace(_patientSearchText) ? _patientSearchText : _lastPatientQuery;
        if (_patientLookup is not null) await _patientLookup.CloseMenuAsync();
        if (_genderOptions.Count == 0)
        {
            var options = await SafeExecute.RunAsync(() => EnumOptionsService.GetOptionsAsync("Gender"), Snackbar, () => T.VisitDialog.Unreachable);
            if (options is null || options.Count == 0) return;
            _genderOptions = options;
        }
        _quickRegistrationOpen = true;
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
            await MudDialog.SetOptionsAsync(MudDialog.Options with { BackdropClick = false, CloseOnEscapeKey = false });
        }
        else if (_optionsBeforePatientSave is { } original)
        {
            _optionsBeforePatientSave = null;
            await MudDialog.SetOptionsAsync(original);
        }
    }

    private Task UseRegisteredPatientAsync(PatientDto patient)
    {
        _model.Patient = patient;
        _patientDraft = null;
        _changingPatient = false;
        _registeredPatientId = patient.Id;
        _editContext.NotifyFieldChanged(_editContext.Field(nameof(VisitFormModel.Patient)));
        _quickRegistrationOpen = false;
        _focusExamination = true;
        Snackbar.Add(T["visitDialog.patientRegistered"], Severity.Success);
        return Task.CompletedTask;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_focusValidation && !_quickRegistrationOpen)
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

    private async Task<IEnumerable<ExaminationTypeDto>> SearchTypesAsync(string? value, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Array.Empty<ExaminationTypeDto>();

        var page = await SafeExecute.RunAsync(
            () => ExaminationService.GetTypesPagedAsync(value, "Name", false, 1, 20, modality: null, isActive: null, ct),
            Snackbar,
            () => T.VisitDialog.SearchTypesError);
        return page?.Items ?? Array.Empty<ExaminationTypeDto>();
    }

    private async Task<IEnumerable<ReferralDoctorDto>> SearchReferralDoctorsAsync(string? value, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Array.Empty<ReferralDoctorDto>();

        var page = await SafeExecute.RunAsync(
            () => ResourceService.GetReferralDoctorsPagedAsync(value, "LastName", false, 1, 20, ct),
            Snackbar,
            () => T.VisitDialog.SearchPatientsError);
        return page?.Items ?? Array.Empty<ReferralDoctorDto>();
    }

    protected override async Task OnInitializedAsync()
    {
        _editContext = new EditContext(_model);
        _scheduleMessages = new ValidationMessageStore(_editContext);
        _editContext.OnFieldChanged += OnEditContextFieldChanged;

        await SafeExecute.RunAsync(async () =>
            {
                _priorityOptions = await EnumOptionsService.GetOptionsAsync("ExaminationPriority");

                var allStatusOptions = await EnumOptionsService.GetOptionsAsync("ExaminationStatus");
                _statusOptions = IsEdit
                    ? BuildEditStatusOptions(allStatusOptions)
                    : allStatusOptions.Where(o => o.Key is "Scheduled" or "CheckedIn").ToList();

                var staffPage = await ResourceService.GetStaffsPagedAsync(null, null, false, 1, 200);
                _radiologists = staffPage.Items
                    .Where(s => s.IsActive && s.PositionKey == "Radiologist")
                    .ToList();
                _technicians = staffPage.Items
                    .Where(s => s.IsActive && s.PositionKey == "Technician")
                    .ToList();
            },
            Snackbar,
            () => T.VisitDialog.Unreachable);

        if (IsEdit)
        {
            _model.Patient = null;
            _model.RadiologistId = Visit!.RadiologistId;
            _model.TechnicianId = Visit.TechnicianId;
            _model.ClinicalIndication = Visit.ClinicalIndication;
            _model.Priority = Visit.PriorityKey;
            _model.Notes = Visit.Notes;
            _model.Discount = Visit.Discount;
            _model.IsDiscountPercentage = Visit.IsDiscountPercentage;
            _model.Paid = Visit.Paid;
            _model.Status = Visit.StatusKey;

            if (Visit.ScheduledAt is { } scheduledLocal)
            {
                _model.ScheduledDate = scheduledLocal.Date;
                _model.ScheduledTime = scheduledLocal.TimeOfDay;
            }

            await SafeExecute.RunAsync(async () =>
                {
                    _model.Patient = await PatientService.GetByIdAsync(Visit.PatientId);
                    _model.ExaminationType = await ExaminationService.GetTypeByIdAsync(Visit.ExaminationTypeId);
                },
                Snackbar,
                () => T.VisitDialog.Unreachable);

            if (!string.IsNullOrWhiteSpace(Visit.ReferralDoctorId))
            {
                await SafeExecute.RunAsync(async () =>
                    _selectedReferralDoctor = await ResourceService.GetReferralDoctorByIdAsync(Visit.ReferralDoctorId!),
                    Snackbar,
                    () => T.VisitDialog.Unreachable);
            }
        }
    }

    private IReadOnlyList<EnumOptionDto> BuildEditStatusOptions(IReadOnlyList<EnumOptionDto> all) =>
        Visit!.StatusKey switch
        {
            "Requested" => all.Where(o => o.Key is "Requested" or "Scheduled" or "CheckedIn").ToList(),
            "Scheduled" => all.Where(o => o.Key is "Scheduled" or "CheckedIn").ToList(),
            _ => all.Where(o => o.Key == Visit!.StatusKey).ToList()
        };

    private bool ValidateSchedule()
    {
        _scheduleMessages!.Clear();
        var ok = true;

        if (_model.Status == "Scheduled")
        {
            if (_model.ScheduledDate is null)
            {
                _scheduleMessages.Add(_editContext.Field(nameof(VisitFormModel.ScheduledDate)), T["validation.scheduledDateRequired"]);
                ok = false;
            }
            if (_model.ScheduledTime is null)
            {
                _scheduleMessages.Add(_editContext.Field(nameof(VisitFormModel.ScheduledTime)), T["validation.scheduledTimeRequired"]);
                ok = false;
            }
        }

        _editContext.NotifyValidationStateChanged();
        return ok;
    }

    private async Task SubmitAsync()
    {
        if (IsBusy || _quickRegistrationOpen) return;
        _submitAttempted = true;
        // Remove previous conditional errors before checking the current status.
        _scheduleMessages!.Clear();
        var baseValid = _editContext.Validate();
        var scheduleValid = ValidateSchedule();
        if (!baseValid || !scheduleValid)
        {
            _focusValidation = true;
            return;
        }

        string? scheduledAt = null;
        if (_model.Status == "Scheduled")
        {
            var local = _model.ScheduledDate!.Value.Date + (_model.ScheduledTime ?? TimeSpan.Zero);
            scheduledAt = local.ToString("O");
        }

        await SafeExecute.RunAsync(async () =>
            {
                if (IsEdit)
                {
                    await ExaminationService.UpdateAsync(Visit!.Id, new ExaminationUpdateInput
                    {
                        PatientId = _model.Patient?.Id,
                        ExaminationTypeId = _model.ExaminationType?.Id,
                        RadiologistId = _model.RadiologistId,
                        TechnicianId = _model.TechnicianId,
                        ReferralDoctorId = _selectedReferralDoctor?.Id,
                        ClinicalIndication = _model.ClinicalIndication,
                        Priority = _model.Priority,
                        Notes = _model.Notes,
                        Discount = _model.Discount,
                        IsDiscountPercentage = _model.IsDiscountPercentage,
                        Paid = _model.Paid,
                        Status = _model.Status,
                        ScheduledAt = scheduledAt,
                    });
                    Snackbar.Add(T.VisitDialog.Updated, Severity.Success);
                }
                else
                {
                    await ExaminationService.CreateAsync(new ExaminationInput
                    {
                        PatientId = _model.Patient!.Id,
                        ExaminationTypeId = _model.ExaminationType!.Id,
                        RadiologistId = _model.RadiologistId,
                        TechnicianId = _model.TechnicianId,
                        ReferralDoctorId = _selectedReferralDoctor?.Id,
                        ClinicalIndication = _model.ClinicalIndication,
                        Priority = _model.Priority,
                        Discount = _model.Discount,
                        IsDiscountPercentage = _model.IsDiscountPercentage,
                        Paid = _model.Paid,
                        Notes = _model.Notes,
                        Status = _model.Status,
                        ScheduledAt = scheduledAt,
                    });
                    Snackbar.Add(T.VisitDialog.Created, Severity.Success);
                }

                MudDialog.Close(DialogResult.Ok(true));
            },
            Snackbar,
            () => T.VisitDialog.Unreachable,
            busy => _busy = busy);
    }

    private void CancelAsync()
    {
        if (!IsBusy) MudDialog.Cancel();
    }
}

