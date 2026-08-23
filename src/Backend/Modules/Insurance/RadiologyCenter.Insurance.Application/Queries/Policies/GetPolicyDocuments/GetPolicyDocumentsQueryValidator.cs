using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Insurance.Application.Queries.Policies.GetPolicyDocuments;

public class GetPolicyDocumentsQueryValidator : AbstractValidator<GetPolicyDocumentsQuery>
{
    public GetPolicyDocumentsQueryValidator()
    {
        RuleFor(x => x.PolicyId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}