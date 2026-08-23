using FluentValidation;
using ErrorCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Commands.DeactivateReferralFee;

public class DeactivateReferralFeeCommandValidator : AbstractValidator<DeactivateReferralFeeCommand>
{
    public DeactivateReferralFeeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired);
    }
}