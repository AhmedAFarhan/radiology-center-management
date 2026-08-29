using FluentValidation;
using RadiologyCenter.ResourceManagement.Application.Commands.Common;
using ErrorCodes = RadiologyCenter.ResourceManagement.Application.Localization.ErrorCodes;

namespace RadiologyCenter.ResourceManagement.Application.Commands.UpdateReferralDoctor;

public class UpdateReferralDoctorCommandValidator : ReferralDoctorValidatorBase<UpdateReferralDoctorCommand>
{
    public UpdateReferralDoctorCommandValidator()
    {
        RuleFor(x => x.ReferralDoctorId).NotEmpty().WithErrorCode(ErrorCodes.ReferralDoctorIdRequired);
    }
}
