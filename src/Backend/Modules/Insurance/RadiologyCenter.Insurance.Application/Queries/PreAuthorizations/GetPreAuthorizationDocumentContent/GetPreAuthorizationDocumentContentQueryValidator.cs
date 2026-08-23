using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Insurance.Application.Queries.PreAuthorizations.GetPreAuthorizationDocumentContent;

public class GetPreAuthorizationDocumentContentQueryValidator : AbstractValidator<GetPreAuthorizationDocumentContentQuery>
{
    public GetPreAuthorizationDocumentContentQueryValidator()
    {
        RuleFor(x => x.PreAuthorizationId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.DocumentId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}