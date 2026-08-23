using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Insurance.Application.Queries.PreAuthorizations.GetPreAuthorizationDocuments;

public class GetPreAuthorizationDocumentsQueryValidator : AbstractValidator<GetPreAuthorizationDocumentsQuery>
{
    public GetPreAuthorizationDocumentsQueryValidator()
    {
        RuleFor(x => x.PreAuthorizationId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}