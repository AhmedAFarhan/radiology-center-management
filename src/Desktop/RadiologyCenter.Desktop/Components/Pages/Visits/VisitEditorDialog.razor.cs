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

public partial class VisitEditorDialog : ComponentBase
{
[Parameter] public ExaminationDto? Visit { get; set; }

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private IReadOnlyList<EnumOptionDto> _priorityOptions = Array.Empty<EnumOptionDto>();
    private IReadOnlyList<StaffDto> _radiologists = Array.Empty<StaffDto>();
    private IReadOnlyList<StaffDto> _technicians = Array.Empty<StaffDto>();

    private readonly VisitFormModel _model = new();
    private EditContext _editContext = default!;
    private ReferralDoctorDto? _selectedReferralDoctor;
    private bool _busy;

    private bool IsEdit => Visit is not null;

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

        await SafeExecute.RunAsync(async () =>
            {
                _priorityOptions = await EnumOptionsService.GetOptionsAsync("ExaminationPriority");

                var staffPage = await ResourceService.GetStaffsPagedAsync(null, null, false, 1, 200);
                _radiologists = staffPage.Items
                    .Where(s => s.IsActive && s.Position == "Radiologist")
                    .ToList();
                _technicians = staffPage.Items
                    .Where(s => s.IsActive && s.Position == "Technician")
                    .ToList();
            },
            Snackbar,
            () => T.VisitDialog.Unreachable);

        if (IsEdit)
        {
            _model.PatientName = T.Common.Loading;
            _model.ExaminationTypeName = Visit!.ExaminationTypeName;
            _model.RadiologistId = Visit.RadiologistId;
            _model.TechnicianId = Visit.TechnicianId;
            _model.ClinicalIndication = Visit.ClinicalIndication;
            _model.Priority = Visit.Priority;
            _model.Notes = Visit.Notes;
            _model.Discount = Visit.Discount;
            _model.IsDiscountPercentage = Visit.IsDiscountPercentage;

            if (!string.IsNullOrWhiteSpace(Visit.ReferralDoctorId))
            {
                await SafeExecute.RunAsync(async () =>
                    _selectedReferralDoctor = await ResourceService.GetReferralDoctorByIdAsync(Visit.ReferralDoctorId!),
                    Snackbar,
                    () => T.VisitDialog.Unreachable);
            }
        }
    }

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        await SafeExecute.RunAsync(async () =>
            {
                if (IsEdit)
                {
                    await ExaminationService.UpdateAsync(Visit!.Id, new ExaminationUpdateInput
                    {
                        RadiologistId = _model.RadiologistId,
                        TechnicianId = _model.TechnicianId,
                        ReferralDoctorId = _selectedReferralDoctor?.Id,
                        ClinicalIndication = _model.ClinicalIndication,
                        Priority = _model.Priority,
                        Notes = _model.Notes,
                        Discount = _model.Discount,
                        IsDiscountPercentage = _model.IsDiscountPercentage,
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
        public string PatientName { get; set; } = string.Empty;
        public string ExaminationTypeName { get; set; } = string.Empty;

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
        public decimal Discount { get; set; }
        public bool IsDiscountPercentage { get; set; }
        public decimal Paid { get; set; }
        public string? Notes { get; set; }
    }
}
