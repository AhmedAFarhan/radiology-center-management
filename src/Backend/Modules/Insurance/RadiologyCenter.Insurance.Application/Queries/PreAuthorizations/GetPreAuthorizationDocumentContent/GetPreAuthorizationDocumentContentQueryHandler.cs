using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Application.Localization;

namespace RadiologyCenter.Insurance.Application.Queries.PreAuthorizations.GetPreAuthorizationDocumentContent;

public static class GetPreAuthorizationDocumentContentQueryHandler
{
    public static async Task<Result<PreAuthorizationDocumentContentDto>> HandleAsync(
        GetPreAuthorizationDocumentContentQuery query,
        IPreAuthorizationDocumentRepository documentRepository,
        IInsuranceDocumentStorage storage,
        CancellationToken ct)
    {
        var document = await documentRepository.GetByIdAsync(query.DocumentId, ct);
        if (document is null || document.PreAuthorizationId != query.PreAuthorizationId)
            return Result.Failure<PreAuthorizationDocumentContentDto>(Error.NotFound(ErrorCodes.PreAuthorizationDocumentNotFound, "PreAuthorizationDocument", query.DocumentId));

        var content = await ReadAllBytesAsync(storage, document.StoredPath, ct);

        return Result.Success(new PreAuthorizationDocumentContentDto(
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