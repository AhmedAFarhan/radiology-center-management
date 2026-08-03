using FluentValidation;

namespace RadiologyCenter.Payroll.Application.Commands.CreateReferralFee;

public class CreateReferralFeeCommandValidator : AbstractValidator<CreateReferralFeeCommand>
{
    public CreateReferralFeeCommandValidator()
    {
        RuleFor(x => x.ReferralDoctorId).NotEmpty();
        RuleFor(x => x.ExaminationTypeId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Amount).LessThanOrEqualTo(100).When(x => x.IsPercentage);
    }
}