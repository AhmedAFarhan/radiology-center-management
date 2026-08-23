using FluentValidation;
using ErrorCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Commands.ActivateReferralFee;

public class ActivateReferralFeeCommandValidator : AbstractValidator<ActivateReferralFeeCommand>
{
    public ActivateReferralFeeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired);
    }
}