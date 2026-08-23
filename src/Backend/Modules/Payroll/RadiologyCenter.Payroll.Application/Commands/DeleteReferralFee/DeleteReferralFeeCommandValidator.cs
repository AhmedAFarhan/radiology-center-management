using FluentValidation;
using ErrorCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Commands.DeleteReferralFee;

public class DeleteReferralFeeCommandValidator : AbstractValidator<DeleteReferralFeeCommand>
{
    public DeleteReferralFeeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired);
    }
}