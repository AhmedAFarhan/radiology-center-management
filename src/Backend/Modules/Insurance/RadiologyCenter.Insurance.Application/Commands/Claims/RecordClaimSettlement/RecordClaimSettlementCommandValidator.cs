using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Insurance.Application.Commands.Claims.RecordClaimSettlement;

public class RecordClaimSettlementCommandValidator : AbstractValidator<RecordClaimSettlementCommand>
{
    public RecordClaimSettlementCommandValidator()
    {
        RuleFor(x => x.ClaimId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.Method).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired);
        RuleFor(x => x.Amount).GreaterThan(0).WithErrorCode(SharedCodes.Shared.ValueMustBePositive);
    }
}