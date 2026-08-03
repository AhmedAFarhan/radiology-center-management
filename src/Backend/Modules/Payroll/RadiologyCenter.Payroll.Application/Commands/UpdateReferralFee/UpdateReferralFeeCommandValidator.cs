using FluentValidation;

namespace RadiologyCenter.Payroll.Application.Commands.UpdateReferralFee;

public class UpdateReferralFeeCommandValidator : AbstractValidator<UpdateReferralFeeCommand>
{
    public UpdateReferralFeeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Amount).LessThanOrEqualTo(100).When(x => x.IsPercentage);
    }
}