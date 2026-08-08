using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Application.DTOs;

namespace RadiologyCenter.Insurance.Application.Queries.Policies.GetPolicyDocuments;

public static class GetPolicyDocumentsQueryHandler
{
    public static async Task<Result<IReadOnlyList<PolicyDocumentDto>>> HandleAsync(
        GetPolicyDocumentsQuery query,
        IPolicyDocumentRepository documentRepository,
        CancellationToken ct)
    {
        var documents = await documentRepository.GetByPolicyIdAsync(query.PolicyId, ct);
        return Result.Success<IReadOnlyList<PolicyDocumentDto>>(
            documents.Select(d => d.ToDto()).ToList());
    }
}