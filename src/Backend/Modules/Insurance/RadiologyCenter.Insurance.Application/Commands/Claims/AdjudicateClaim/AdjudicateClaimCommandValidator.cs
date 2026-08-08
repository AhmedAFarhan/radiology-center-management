using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Commands.Claims.AdjudicateClaim;

public class AdjudicateClaimCommandValidator : AbstractValidator<AdjudicateClaimCommand>
{
    public AdjudicateClaimCommandValidator()
    {
        RuleFor(x => x.ClaimId).NotEmpty();
        RuleFor(x => x.Decision).IsInEnum();
        RuleFor(x => x.ApprovedAmount).GreaterThanOrEqualTo(0).When(x => x.ApprovedAmount.HasValue);
        RuleFor(x => x.RejectionCode)
            .NotEmpty()
            .When(x => x.Decision == ClaimAdjudication.Reject);
        RuleFor(x => x.RejectionReason)
            .NotEmpty()
            .When(x => x.Decision == ClaimAdjudication.Reject);
    }
}