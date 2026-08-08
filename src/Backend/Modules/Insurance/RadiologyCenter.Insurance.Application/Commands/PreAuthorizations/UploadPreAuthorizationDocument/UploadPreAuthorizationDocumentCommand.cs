namespace RadiologyCenter.Insurance.Application.Commands.PreAuthorizations.UploadPreAuthorizationDocument;

public record UploadPreAuthorizationDocumentCommand(
    Guid PreAuthorizationId,
    string Type,
    string FileName,
    string ContentType,
    long SizeInBytes,
    Stream Content) : ICommand;