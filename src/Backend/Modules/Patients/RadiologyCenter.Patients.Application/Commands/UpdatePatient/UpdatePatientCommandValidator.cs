using FluentValidation;
using RadiologyCenter.Patients.Application.Commands.Common;

namespace RadiologyCenter.Patients.Application.Commands.UpdatePatient;

public class UpdatePatientCommandValidator : PatientValidatorBase<UpdatePatientCommand>
{
    public UpdatePatientCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
    }
}
