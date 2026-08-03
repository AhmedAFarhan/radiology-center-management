using FluentValidation;
using RadiologyCenter.ResourceManagement.Application.Commands.Common;

namespace RadiologyCenter.ResourceManagement.Application.Commands.UpdateReferralDoctor;

public class UpdateReferralDoctorCommandValidator : ReferralDoctorValidatorBase<UpdateReferralDoctorCommand>
{
    public UpdateReferralDoctorCommandValidator()
    {
        RuleFor(x => x.ReferralDoctorId).NotEmpty();
    }
}
