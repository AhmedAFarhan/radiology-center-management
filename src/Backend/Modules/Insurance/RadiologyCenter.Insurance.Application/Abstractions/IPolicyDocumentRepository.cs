using RadiologyCenter.BuildingBlocks.Application.Abstractions;

namespace RadiologyCenter.Insurance.Application.Abstractions;

public interface IPolicyDocumentRepository : IBaseRepository<PolicyDocument, Guid>
{
    Task<IReadOnlyList<PolicyDocument>> GetByPolicyIdAsync(Guid policyId, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid policyId, DocumentType type, CancellationToken ct = default);
}