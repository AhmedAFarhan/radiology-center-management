using FluentValidation;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Commands.UpdateReferralFee;

public class UpdateReferralFeeCommandValidator : AbstractValidator<UpdateReferralFeeCommand>
{
    public UpdateReferralFeeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.ReferralFeeIdRequired);
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.AmountCannotBeNegative);
        RuleFor(x => x.Amount).LessThanOrEqualTo(100).WithErrorCode(ErrorCodes.PercentageAmountMax).When(x => x.IsPercentage);
    }
}