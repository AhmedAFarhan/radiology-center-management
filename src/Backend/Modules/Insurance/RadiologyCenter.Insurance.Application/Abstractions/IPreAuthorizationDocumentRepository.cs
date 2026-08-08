using RadiologyCenter.BuildingBlocks.Application.Abstractions;

namespace RadiologyCenter.Insurance.Application.Abstractions;

public interface IPreAuthorizationDocumentRepository : IBaseRepository<PreAuthorizationDocument, Guid>
{
    Task<IReadOnlyList<PreAuthorizationDocument>> GetByPreAuthorizationIdAsync(Guid preAuthorizationId, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid preAuthorizationId, DocumentType type, CancellationToken ct = default);
}