using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Commands.Claims.SubmitClaim;

public class SubmitClaimCommandValidator : AbstractValidator<SubmitClaimCommand>
{
    public SubmitClaimCommandValidator()
    {
        RuleFor(x => x.ClaimId).NotEmpty();
    }
}