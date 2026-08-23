using FluentValidation;
using RadiologyCenter.Payroll.Application.Localization;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Commands.CreateReferralFee;

public class CreateReferralFeeCommandValidator : AbstractValidator<CreateReferralFeeCommand>
{
    public CreateReferralFeeCommandValidator()
    {
        RuleFor(x => x.ReferralDoctorId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.ExaminationTypeId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0).WithErrorCode(SharedCodes.Shared.CannotBeNegative);
        RuleFor(x => x.Amount).LessThanOrEqualTo(100).WithErrorCode(ErrorCodes.PercentageAmountMax).When(x => x.IsPercentage);
    }
}