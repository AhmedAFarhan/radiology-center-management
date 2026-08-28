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

namespace RadiologyCenter.Desktop.Features.Resources.Pages;

public partial class ReferralDoctorEditorDialog : EditorDialogBase
{
    [Parameter] public ReferralDoctorDto? Doctor { get; set; }

    private readonly ReferralDoctorFormModel _model = new();
    private EditContext _editContext = default!;

    private bool IsEdit => Doctor is not null;

    protected override void OnInitialized()
    {
        _editContext = new EditContext(_model);

        if (Doctor is null)
            return;

        _model.FullName = Doctor.FullName;
        _model.Phone = Doctor.Phone;
        _model.Email = Doctor.Email;
        _model.Specialization = Doctor.Specialization;
        _model.Hospital = Doctor.Hospital;
    }

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        var input = new ReferralDoctorInput
        {
            FullName = _model.FullName,
            Phone = _model.Phone,
            Email = _model.Email,
            Specialization = _model.Specialization,
            Hospital = _model.Hospital,
        };

        if (await TrySaveAsync(
                () => IsEdit
                    ? ResourceService.UpdateReferralDoctorAsync(Doctor!.Id, input)
                    : ResourceService.CreateReferralDoctorAsync(input),
                () => T.ReferralDoctorDialog.Unreachable))
        {
            Snackbar.Add(IsEdit ? T.ReferralDoctorDialog.Updated : T.ReferralDoctorDialog.Created, Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
    }

    private sealed class ReferralDoctorFormModel : IValidatableObject
    {
        [Required(ErrorMessage = "Full name is required.")]
        [MaxLength(300, ErrorMessage = "Full name must be 300 characters or fewer.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [MaxLength(30, ErrorMessage = "Phone number must be 30 characters or fewer.")]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = "Email must be 200 characters or fewer.")]
        public string? Email { get; set; }

        [MaxLength(200, ErrorMessage = "Specialization must be 200 characters or fewer.")]
        public string? Specialization { get; set; }

        [MaxLength(200, ErrorMessage = "Hospital must be 200 characters or fewer.")]
        public string? Hospital { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Length < 2)
                yield return new ValidationResult("Full name must include at least a first and last name.", new[] { nameof(FullName) });

            if (!string.IsNullOrWhiteSpace(Phone) && !EgyptianPhoneNumber.IsValid(Phone))
                yield return new ValidationResult("Phone number must be a valid Egyptian number (e.g. 01012345678).", new[] { nameof(Phone) });
        }
    }
}
