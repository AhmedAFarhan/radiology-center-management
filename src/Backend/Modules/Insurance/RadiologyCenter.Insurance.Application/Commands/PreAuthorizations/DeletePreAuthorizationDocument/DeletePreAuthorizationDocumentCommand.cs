namespace RadiologyCenter.Insurance.Application.Commands.PreAuthorizations.DeletePreAuthorizationDocument;

public record DeletePreAuthorizationDocumentCommand(
    Guid PreAuthorizationId,
    Guid DocumentId) : ICommand;