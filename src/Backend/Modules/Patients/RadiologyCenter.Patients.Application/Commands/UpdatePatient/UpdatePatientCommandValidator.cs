using FluentValidation;
using RadiologyCenter.Patients.Application.Commands.Common;
using RadiologyCenter.Patients.Application.Localization;

namespace RadiologyCenter.Patients.Application.Commands.UpdatePatient;

public class UpdatePatientCommandValidator : PatientValidatorBase<UpdatePatientCommand>
{
    public UpdatePatientCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty().WithErrorCode(ErrorCodes.PatientIdRequired);
    }
}
