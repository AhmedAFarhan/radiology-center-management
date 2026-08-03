using FluentValidation;

namespace RadiologyCenter.Payroll.Application.Commands.DeactivateReferralFee;

public class DeactivateReferralFeeCommandValidator : AbstractValidator<DeactivateReferralFeeCommand>
{
    public DeactivateReferralFeeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}