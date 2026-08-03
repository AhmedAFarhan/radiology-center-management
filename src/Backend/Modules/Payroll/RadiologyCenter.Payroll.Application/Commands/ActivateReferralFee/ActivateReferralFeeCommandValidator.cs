using FluentValidation;

namespace RadiologyCenter.Payroll.Application.Commands.ActivateReferralFee;

public class ActivateReferralFeeCommandValidator : AbstractValidator<ActivateReferralFeeCommand>
{
    public ActivateReferralFeeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}