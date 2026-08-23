using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Insurance.Application.Commands.Claims.ResubmitClaim;

public class ResubmitClaimCommandValidator : AbstractValidator<ResubmitClaimCommand>
{
    public ResubmitClaimCommandValidator()
    {
        RuleFor(x => x.ClaimId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}