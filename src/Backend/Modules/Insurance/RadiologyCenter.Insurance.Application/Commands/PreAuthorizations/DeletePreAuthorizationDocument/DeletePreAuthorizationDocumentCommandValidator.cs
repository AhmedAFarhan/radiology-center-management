using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Insurance.Application.Commands.PreAuthorizations.DeletePreAuthorizationDocument;

public class DeletePreAuthorizationDocumentCommandValidator : AbstractValidator<DeletePreAuthorizationDocumentCommand>
{
    public DeletePreAuthorizationDocumentCommandValidator()
    {
        RuleFor(x => x.PreAuthorizationId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.DocumentId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}