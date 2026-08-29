using FluentValidation;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Commands.ActivateReferralFee;

public class ActivateReferralFeeCommandValidator : AbstractValidator<ActivateReferralFeeCommand>
{
    public ActivateReferralFeeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.ReferralFeeIdRequired);
    }
}