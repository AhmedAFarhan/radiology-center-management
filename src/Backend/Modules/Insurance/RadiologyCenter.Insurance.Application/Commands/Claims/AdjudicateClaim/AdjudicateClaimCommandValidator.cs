using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Insurance.Application.Commands.Claims.AdjudicateClaim;

public class AdjudicateClaimCommandValidator : AbstractValidator<AdjudicateClaimCommand>
{
    public AdjudicateClaimCommandValidator()
    {
        RuleFor(x => x.ClaimId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.Decision).IsInEnum().WithErrorCode(SharedCodes.Shared.InvalidEnumValue);
        RuleFor(x => x.ApprovedAmount).GreaterThanOrEqualTo(0).WithErrorCode(SharedCodes.Shared.CannotBeNegative).When(x => x.ApprovedAmount.HasValue);
        RuleFor(x => x.RejectionCode)
            .NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired)
            .When(x => x.Decision == ClaimAdjudication.Reject);
        RuleFor(x => x.RejectionReason)
            .NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired)
            .When(x => x.Decision == ClaimAdjudication.Reject);
    }
}