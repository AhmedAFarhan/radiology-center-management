using FluentValidation;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Commands.CreateReferralFee;

public class CreateReferralFeeCommandValidator : AbstractValidator<CreateReferralFeeCommand>
{
    public CreateReferralFeeCommandValidator()
    {
        RuleFor(x => x.ReferralDoctorId).NotEmpty().WithErrorCode(ErrorCodes.ReferralDoctorIdRequired);
        RuleFor(x => x.ExaminationTypeId).NotEmpty().WithErrorCode(ErrorCodes.ExaminationTypeIdRequired);
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.AmountCannotBeNegative);
        RuleFor(x => x.Amount).LessThanOrEqualTo(100).WithErrorCode(ErrorCodes.PercentageAmountMax).When(x => x.IsPercentage);
    }
}