using System.ComponentModel.DataAnnotations;

using RadiologyCenter.Desktop.Features.Patients.Models;

namespace RadiologyCenter.Desktop.Features.Patients.Components;

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

}
