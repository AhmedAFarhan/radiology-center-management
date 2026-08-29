using FluentValidation;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Commands.DeleteReferralFee;

public class DeleteReferralFeeCommandValidator : AbstractValidator<DeleteReferralFeeCommand>
{
    public DeleteReferralFeeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.ReferralFeeIdRequired);
    }
}