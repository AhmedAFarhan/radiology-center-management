using FluentValidation;
using RadiologyCenter.ResourceManagement.Application.Commands.Common;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.ResourceManagement.Application.Commands.UpdateReferralDoctor;

public class UpdateReferralDoctorCommandValidator : ReferralDoctorValidatorBase<UpdateReferralDoctorCommand>
{
    public UpdateReferralDoctorCommandValidator()
    {
        RuleFor(x => x.ReferralDoctorId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}
