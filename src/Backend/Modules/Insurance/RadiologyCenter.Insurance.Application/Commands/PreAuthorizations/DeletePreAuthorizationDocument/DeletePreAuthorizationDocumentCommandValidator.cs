using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Commands.PreAuthorizations.DeletePreAuthorizationDocument;

public class DeletePreAuthorizationDocumentCommandValidator : AbstractValidator<DeletePreAuthorizationDocumentCommand>
{
    public DeletePreAuthorizationDocumentCommandValidator()
    {
        RuleFor(x => x.PreAuthorizationId).NotEmpty().WithErrorCode(ErrorCodes.PreAuthorizationIdRequired);
        RuleFor(x => x.DocumentId).NotEmpty().WithErrorCode(ErrorCodes.DocumentIdRequired);
    }
}
