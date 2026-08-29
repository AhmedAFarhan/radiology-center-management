using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Queries.Policies.GetPolicyDocumentContent;

public class GetPolicyDocumentContentQueryValidator : AbstractValidator<GetPolicyDocumentContentQuery>
{
    public GetPolicyDocumentContentQueryValidator()
    {
        RuleFor(x => x.PolicyId).NotEmpty().WithErrorCode(ErrorCodes.PolicyIdRequired);
        RuleFor(x => x.DocumentId).NotEmpty().WithErrorCode(ErrorCodes.DocumentIdRequired);
    }
}
