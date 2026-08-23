using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Insurance.Application.Queries.Policies.GetPolicyDocumentContent;

public class GetPolicyDocumentContentQueryValidator : AbstractValidator<GetPolicyDocumentContentQuery>
{
    public GetPolicyDocumentContentQueryValidator()
    {
        RuleFor(x => x.PolicyId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.DocumentId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}