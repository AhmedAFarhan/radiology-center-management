using FluentValidation;
using RadiologyCenter.Payroll.Application.Localization;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Commands.UpdateReferralFee;

public class UpdateReferralFeeCommandValidator : AbstractValidator<UpdateReferralFeeCommand>
{
    public UpdateReferralFeeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired);
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0).WithErrorCode(SharedCodes.Shared.CannotBeNegative);
        RuleFor(x => x.Amount).LessThanOrEqualTo(100).WithErrorCode(ErrorCodes.PercentageAmountMax).When(x => x.IsPercentage);
    }
}