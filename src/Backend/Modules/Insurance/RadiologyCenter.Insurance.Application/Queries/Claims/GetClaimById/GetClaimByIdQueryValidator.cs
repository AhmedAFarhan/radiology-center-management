using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Insurance.Application.Queries.Claims.GetClaimById;

public class GetClaimByIdQueryValidator : AbstractValidator<GetClaimByIdQuery>
{
    public GetClaimByIdQueryValidator()
    {
        RuleFor(x => x.ClaimId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}