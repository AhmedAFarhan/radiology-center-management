using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Application.DTOs;

namespace RadiologyCenter.Insurance.Application.Commands.PreAuthorizations.UploadPreAuthorizationDocument;

public static class UploadPreAuthorizationDocumentCommandHandler
{
    public static async Task<Result<PreAuthorizationDocumentDto>> HandleAsync(
        UploadPreAuthorizationDocumentCommand command,
        IPreAuthorizationRepository preAuthorizationRepository,
        IInsuranceDocumentStorage storage,
        IInsuranceUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var preAuthorization = await preAuthorizationRepository.GetByIdAsync(command.PreAuthorizationId, ct);
        if (preAuthorization is null)
            return Result.Failure<PreAuthorizationDocumentDto>(Error.NotFound("PreAuthorization", command.PreAuthorizationId));

        var type = DocumentType.FromName<DocumentType>(command.Type);
        if (type is null)
            return Result.Failure<PreAuthorizationDocumentDto>(Error.Validation("Type", $"'{command.Type}' is not a valid document type."));

        var relativeDirectory = Path.Combine("preauthorizations", command.PreAuthorizationId.ToString());
        var storedPath = await storage.SaveAsync(relativeDirectory, command.FileName, command.Content, ct);

        var document = preAuthorization.AddDocument(type, command.FileName, command.ContentType, storedPath, command.SizeInBytes);

        preAuthorizationRepository.Update(preAuthorization);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(document.ToDto());
    }
}