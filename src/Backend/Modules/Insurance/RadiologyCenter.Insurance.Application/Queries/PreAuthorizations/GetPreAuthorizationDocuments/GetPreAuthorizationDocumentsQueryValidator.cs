using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Queries.PreAuthorizations.GetPreAuthorizationDocuments;

public class GetPreAuthorizationDocumentsQueryValidator : AbstractValidator<GetPreAuthorizationDocumentsQuery>
{
    public GetPreAuthorizationDocumentsQueryValidator()
    {
        RuleFor(x => x.PreAuthorizationId).NotEmpty();
    }
}