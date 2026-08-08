using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Commands.Claims.ResubmitClaim;

public class ResubmitClaimCommandValidator : AbstractValidator<ResubmitClaimCommand>
{
    public ResubmitClaimCommandValidator()
    {
        RuleFor(x => x.ClaimId).NotEmpty();
    }
}