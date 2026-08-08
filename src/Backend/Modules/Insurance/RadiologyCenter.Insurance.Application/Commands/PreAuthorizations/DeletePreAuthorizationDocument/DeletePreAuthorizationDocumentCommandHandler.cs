using RadiologyCenter.Insurance.Application.Abstractions;

namespace RadiologyCenter.Insurance.Application.Commands.PreAuthorizations.DeletePreAuthorizationDocument;

public static class DeletePreAuthorizationDocumentCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeletePreAuthorizationDocumentCommand command,
        IPreAuthorizationDocumentRepository documentRepository,
        IInsuranceDocumentStorage storage,
        IInsuranceUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var document = await documentRepository.GetByIdAsync(command.DocumentId, ct);
        if (document is null || document.PreAuthorizationId != command.PreAuthorizationId)
            return Result.Failure(Error.NotFound("PreAuthorizationDocument", command.DocumentId));

        storage.Delete(document.StoredPath);

        documentRepository.Remove(document);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}