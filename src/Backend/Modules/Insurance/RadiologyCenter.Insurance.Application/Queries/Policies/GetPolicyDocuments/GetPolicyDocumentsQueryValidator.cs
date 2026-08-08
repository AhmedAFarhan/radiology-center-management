using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Queries.Policies.GetPolicyDocuments;

public class GetPolicyDocumentsQueryValidator : AbstractValidator<GetPolicyDocumentsQuery>
{
    public GetPolicyDocumentsQueryValidator()
    {
        RuleFor(x => x.PolicyId).NotEmpty();
    }
}