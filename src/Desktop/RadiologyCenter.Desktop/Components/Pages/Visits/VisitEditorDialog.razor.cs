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
using RadiologyCenter.Desktop.Components;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Components.Pages.Visits;

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
        if (e.FieldIdentifier.FieldName is nameof(VisitFormModel.ScheduledDate) or nameof(VisitFormModel.ScheduledTime))
            ValidateSchedule();
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
        if (string.IsNullOrWhiteSpace(value))
            return Array.Empty<PatientDto>();

        try
        {
            var page = await PatientService.GetPagedAsync(value, "LastName", false, 1, 20, ct);
            return page.Items;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            return Array.Empty<PatientDto>();
        }
        catch (Exception)
        {
            Snackbar.Add(T.VisitDialog.SearchPatientsError, Severity.Error);
            return Array.Empty<PatientDto>();
        }
    }

    private async Task<IEnumerable<ExaminationTypeDto>> SearchTypesAsync(string? value, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Array.Empty<ExaminationTypeDto>();

        try
        {
            var page = await ExaminationService.GetTypesPagedAsync(value, "Name", false, 1, 20, ct);
            return page.Items;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            return Array.Empty<ExaminationTypeDto>();
        }
        catch (Exception)
        {
            Snackbar.Add(T.VisitDialog.SearchTypesError, Severity.Error);
            return Array.Empty<ExaminationTypeDto>();
        }
    }

    private async Task<IEnumerable<ReferralDoctorDto>> SearchReferralDoctorsAsync(string? value, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Array.Empty<ReferralDoctorDto>();

        try
        {
            var page = await ResourceService.GetReferralDoctorsPagedAsync(value, "LastName", false, 1, 20, ct);
            return page.Items;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            return Array.Empty<ReferralDoctorDto>();
        }
        catch (Exception)
        {
            Snackbar.Add(T.VisitDialog.SearchPatientsError, Severity.Error);
            return Array.Empty<ReferralDoctorDto>();
        }
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
            _model.Status = Visit.StatusKey;

            if (Visit.ScheduledAt is { } scheduledUtc)
            {
                var local = ToClinicLocal(scheduledUtc);
                _model.ScheduledDate = local.Date;
                _model.ScheduledTime = local.TimeOfDay;
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
        var baseValid = _editContext.Validate();
        var scheduleValid = ValidateSchedule();
        if (!baseValid || !scheduleValid)
            return;

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
        => MudDialog.Cancel();

    private sealed class VisitFormModel
    {
        public string? RadiologistId { get; set; }

        public string? TechnicianId { get; set; }

        [Required(ErrorMessage = "Patient is required.")]
        public PatientDto? Patient { get; set; }

        [Required(ErrorMessage = "Examination type is required.")]
        public ExaminationTypeDto? ExaminationType { get; set; }

        [Required(ErrorMessage = "Clinical indication is required.")]
        [MaxLength(1000, ErrorMessage = "Clinical indication must be 1000 characters or fewer.")]
        public string ClinicalIndication { get; set; } = string.Empty;

        public string Priority { get; set; } = "Routine";
        public string Status { get; set; } = "Scheduled";

        public DateTime? ScheduledDate { get; set; }

        public TimeSpan? ScheduledTime { get; set; }

        public decimal Discount { get; set; }
        public bool IsDiscountPercentage { get; set; }
        public decimal Paid { get; set; }
        public string? Notes { get; set; }
    }
}
