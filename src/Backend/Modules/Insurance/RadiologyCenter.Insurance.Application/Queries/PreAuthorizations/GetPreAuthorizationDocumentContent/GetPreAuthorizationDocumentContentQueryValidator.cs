using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Queries.PreAuthorizations.GetPreAuthorizationDocumentContent;

public class GetPreAuthorizationDocumentContentQueryValidator : AbstractValidator<GetPreAuthorizationDocumentContentQuery>
{
    public GetPreAuthorizationDocumentContentQueryValidator()
    {
        RuleFor(x => x.PreAuthorizationId).NotEmpty().WithErrorCode(ErrorCodes.PreAuthorizationIdRequired);
        RuleFor(x => x.DocumentId).NotEmpty().WithErrorCode(ErrorCodes.DocumentIdRequired);
    }
}
