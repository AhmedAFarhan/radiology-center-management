namespace RadiologyCenter.Insurance.Application.Queries.PreAuthorizations.GetPreAuthorizationDocumentContent;

public sealed record PreAuthorizationDocumentContentDto(
    Guid DocumentId,
    string FileName,
    string ContentType,
    byte[]? Content);

public record GetPreAuthorizationDocumentContentQuery(Guid PreAuthorizationId, Guid DocumentId) : IQuery;