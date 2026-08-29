using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Commands.Claims.RecordClaimSettlement;

public class RecordClaimSettlementCommandValidator : AbstractValidator<RecordClaimSettlementCommand>
{
    public RecordClaimSettlementCommandValidator()
    {
        RuleFor(x => x.ClaimId).NotEmpty().WithErrorCode(ErrorCodes.ClaimIdRequired);
        RuleFor(x => x.Method).NotEmpty().WithErrorCode(ErrorCodes.SettlementMethodRequired);
        RuleFor(x => x.Amount).GreaterThan(0).WithErrorCode(ErrorCodes.SettlementAmountMustBePositive);
    }
}
