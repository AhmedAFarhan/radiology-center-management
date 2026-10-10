using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Application.Localization;

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
            return Result.Failure(Error.NotFound(ErrorCodes.PreAuthorizationDocumentNotFound, "PreAuthorizationDocument", command.DocumentId));

        documentRepository.Remove(document);
        await unitOfWork.SaveChangesAsync(ct);
        // Retain the file with soft-deleted metadata for the configured backup/retention policy.

        return Result.Success();
    }
}