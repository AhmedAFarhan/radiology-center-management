namespace RadiologyCenter.Insurance.Application.Queries.Policies.GetPolicyDocumentContent;

public sealed record PolicyDocumentContentDto(
    Guid DocumentId,
    string FileName,
    string ContentType,
    byte[]? Content);

public record GetPolicyDocumentContentQuery(Guid PolicyId, Guid DocumentId) : IQuery;