using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Commands.Claims.AdjudicateClaim;

public class AdjudicateClaimCommandValidator : AbstractValidator<AdjudicateClaimCommand>
{
    public AdjudicateClaimCommandValidator()
    {
        RuleFor(x => x.ClaimId).NotEmpty().WithErrorCode(ErrorCodes.ClaimIdRequired);
        RuleFor(x => x.Decision).IsInEnum().WithErrorCode(ErrorCodes.DecisionInvalid);
        RuleFor(x => x.ApprovedAmount).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.ApprovedAmountCannotBeNegative).When(x => x.ApprovedAmount.HasValue);
        RuleFor(x => x.RejectionCode)
            .NotEmpty().WithErrorCode(ErrorCodes.RejectionCodeRequired)
            .When(x => x.Decision == ClaimAdjudication.Reject);
        RuleFor(x => x.RejectionReason)
            .NotEmpty().WithErrorCode(ErrorCodes.RejectionReasonRequired)
            .When(x => x.Decision == ClaimAdjudication.Reject);
    }
}
