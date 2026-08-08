using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Commands.Claims.RecordClaimSettlement;

public class RecordClaimSettlementCommandValidator : AbstractValidator<RecordClaimSettlementCommand>
{
    public RecordClaimSettlementCommandValidator()
    {
        RuleFor(x => x.ClaimId).NotEmpty();
        RuleFor(x => x.Method).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}