using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Commands.PreAuthorizations.DeletePreAuthorizationDocument;

public class DeletePreAuthorizationDocumentCommandValidator : AbstractValidator<DeletePreAuthorizationDocumentCommand>
{
    public DeletePreAuthorizationDocumentCommandValidator()
    {
        RuleFor(x => x.PreAuthorizationId).NotEmpty();
        RuleFor(x => x.DocumentId).NotEmpty();
    }
}