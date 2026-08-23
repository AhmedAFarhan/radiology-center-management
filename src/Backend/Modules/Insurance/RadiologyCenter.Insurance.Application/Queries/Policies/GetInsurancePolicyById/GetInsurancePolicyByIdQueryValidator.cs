using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Insurance.Application.Queries.Policies.GetInsurancePolicyById;

public class GetInsurancePolicyByIdQueryValidator : AbstractValidator<GetInsurancePolicyByIdQuery>
{
    public GetInsurancePolicyByIdQueryValidator()
    {
        RuleFor(x => x.PolicyId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}