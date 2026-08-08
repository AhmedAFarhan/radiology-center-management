using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Queries.Policies.GetInsurancePolicyById;

public class GetInsurancePolicyByIdQueryValidator : AbstractValidator<GetInsurancePolicyByIdQuery>
{
    public GetInsurancePolicyByIdQueryValidator()
    {
        RuleFor(x => x.PolicyId).NotEmpty();
    }
}