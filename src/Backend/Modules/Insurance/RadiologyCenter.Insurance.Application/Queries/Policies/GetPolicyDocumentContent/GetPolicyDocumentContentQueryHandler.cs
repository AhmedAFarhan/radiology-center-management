using RadiologyCenter.Insurance.Application.Abstractions;

namespace RadiologyCenter.Insurance.Application.Queries.Policies.GetPolicyDocumentContent;

public static class GetPolicyDocumentContentQueryHandler
{
    public static async Task<Result<PolicyDocumentContentDto>> HandleAsync(
        GetPolicyDocumentContentQuery query,
        IPolicyDocumentRepository documentRepository,
        IInsuranceDocumentStorage storage,
        CancellationToken ct)
    {
        var document = await documentRepository.GetByIdAsync(query.DocumentId, ct);
        if (document is null || document.PolicyId != query.PolicyId)
            return Result.Failure<PolicyDocumentContentDto>(Error.NotFound("PolicyDocument", query.DocumentId));

        var content = await ReadAllBytesAsync(storage, document.StoredPath, ct);

        return Result.Success(new PolicyDocumentContentDto(
            document.Id,
            document.FileName,
            document.ContentType,
            content));
    }

    private static async Task<byte[]?> ReadAllBytesAsync(
        IInsuranceDocumentStorage storage,
        string storedPath,
        CancellationToken ct)
    {
        if (!storage.Exists(storedPath))
            return null;

        await using var stream = await storage.OpenAsync(storedPath, ct);
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms, ct);
        return ms.ToArray();
    }
}