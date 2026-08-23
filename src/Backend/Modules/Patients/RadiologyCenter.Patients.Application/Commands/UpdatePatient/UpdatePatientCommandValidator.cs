using FluentValidation;
using RadiologyCenter.Patients.Application.Commands.Common;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Patients.Application.Commands.UpdatePatient;

public class UpdatePatientCommandValidator : PatientValidatorBase<UpdatePatientCommand>
{
    public UpdatePatientCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}
