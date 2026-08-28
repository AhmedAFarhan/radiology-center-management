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

namespace RadiologyCenter.Desktop.Features.Patients.Pages;

public partial class PatientEditorDialog : ComponentBase
{
[Parameter] public PatientDto? Patient { get; set; }

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private IReadOnlyList<EnumOptionDto> _genderOptions = Array.Empty<EnumOptionDto>();
    private IReadOnlyList<EnumOptionDto> _bloodTypeOptions = Array.Empty<EnumOptionDto>();

    private readonly PatientFormModel _model = new();
    private EditContext _editContext = default!;
    private bool _busy;

    private bool IsEdit => Patient is not null;

    protected override async Task OnInitializedAsync()
    {
        _editContext = new EditContext(_model);

        await SafeExecute.RunAsync(async () =>
            {
                _genderOptions = await EnumOptionsService.GetOptionsAsync("Gender");
                _bloodTypeOptions = await EnumOptionsService.GetOptionsAsync("BloodType");
            },
            Snackbar,
            () => T.PatientDialog.Unreachable);

        if (Patient is null)
            return;

        _model.FullName = Patient.FullName;
        _model.Gender = Patient.GenderKey;
        _model.DateOfBirth = Patient.DateOfBirth;
        _model.Age = Patient.Age;
        _model.PhoneNumber = Patient.PhoneNumber;
        _model.Email = Patient.Email;
        _model.Address = Patient.Address;
        _model.NationalId = Patient.NationalId;
        _model.BloodType = Patient.BloodTypeKey;
        _model.Allergies = Patient.Allergies;
        _model.MedicalHistory = Patient.MedicalHistory;
    }

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        var input = new PatientInput
        {
            FullName = _model.FullName,
            Gender = _model.Gender,
            DateOfBirth = _model.DateOfBirth,
            Age = _model.Age,
            PhoneNumber = _model.PhoneNumber,
            Email = _model.Email,
            Address = _model.Address,
            NationalId = _model.NationalId,
            BloodType = _model.BloodType,
            Allergies = _model.Allergies,
            MedicalHistory = _model.MedicalHistory,
        };

        if (await SafeExecute.RunAsync(
                () => IsEdit
                    ? PatientService.UpdateAsync(Patient!.Id, input)
                    : PatientService.CreateAsync(input),
                Snackbar,
                () => T.PatientDialog.Unreachable,
                busy => _busy = busy))
        {
            Snackbar.Add(IsEdit ? T.PatientDialog.Updated : T.PatientDialog.Created, Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
    }

    private void CancelAsync()
        => MudDialog.Cancel();

    private sealed class PatientFormModel : IValidatableObject
    {
        [Required(ErrorMessage = "Full name is required.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }
        public int? Age { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string? Email { get; set; }

        public string? Address { get; set; }
        public string? NationalId { get; set; }
        public string? BloodType { get; set; }
        public string? Allergies { get; set; }
        public string? MedicalHistory { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DateOfBirth is null && Age is null)
                yield return new ValidationResult(
                    "Either date of birth or age must be provided.",
                    new[] { nameof(DateOfBirth) });

            if (DateOfBirth is not null && DateOfBirth.Value.Date > DateTime.UtcNow.Date)
                yield return new ValidationResult(
                    "Date of birth cannot be in the future.",
                    new[] { nameof(DateOfBirth) });

            if (Age is not null && Age.Value is < 0 or > 150)
                yield return new ValidationResult(
                    "Age must be between 0 and 150.",
                    new[] { nameof(Age) });

            var parts = FullName?.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts is null || parts.Length < 2)
                yield return new ValidationResult(
                    "Full name must contain at least a first name and a last name.",
                    new[] { nameof(FullName) });
        }
    }
}
