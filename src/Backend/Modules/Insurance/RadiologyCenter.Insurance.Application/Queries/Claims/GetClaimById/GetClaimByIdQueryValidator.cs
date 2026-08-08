using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Queries.Claims.GetClaimById;

public class GetClaimByIdQueryValidator : AbstractValidator<GetClaimByIdQuery>
{
    public GetClaimByIdQueryValidator()
    {
        RuleFor(x => x.ClaimId).NotEmpty();
    }
}