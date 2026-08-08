using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Application.DTOs;

namespace RadiologyCenter.Insurance.Application.Queries.PreAuthorizations.GetPreAuthorizationDocuments;

public static class GetPreAuthorizationDocumentsQueryHandler
{
    public static async Task<Result<IReadOnlyList<PreAuthorizationDocumentDto>>> HandleAsync(
        GetPreAuthorizationDocumentsQuery query,
        IPreAuthorizationDocumentRepository documentRepository,
        CancellationToken ct)
    {
        var documents = await documentRepository.GetByPreAuthorizationIdAsync(query.PreAuthorizationId, ct);
        return Result.Success<IReadOnlyList<PreAuthorizationDocumentDto>>(
            documents.Select(d => d.ToDto()).ToList());
    }
}