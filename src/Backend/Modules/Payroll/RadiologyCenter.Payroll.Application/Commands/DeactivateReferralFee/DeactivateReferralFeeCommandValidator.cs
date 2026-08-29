using FluentValidation;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Commands.DeactivateReferralFee;

public class DeactivateReferralFeeCommandValidator : AbstractValidator<DeactivateReferralFeeCommand>
{
    public DeactivateReferralFeeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.ReferralFeeIdRequired);
    }
}