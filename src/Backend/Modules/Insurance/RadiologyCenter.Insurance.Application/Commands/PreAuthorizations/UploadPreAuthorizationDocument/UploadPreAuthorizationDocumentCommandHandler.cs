using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Application.Localization;
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
            return Result.Failure<PreAuthorizationDocumentDto>(Error.NotFound(ErrorCodes.PreAuthorizationNotFound, "PreAuthorization", command.PreAuthorizationId));

        var type = DocumentType.FromName<DocumentType>(command.Type);
        if (type is null)
            return Result.Failure<PreAuthorizationDocumentDto>(Error.Validation(ErrorCodes.InvalidDocumentType, $"'{command.Type}' is not a valid document type."));

        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);
        var relativeDirectory = Path.Combine("preauthorizations", command.PreAuthorizationId.ToString());
        var storedPath = await storage.SaveAsync(relativeDirectory, command.FileName, command.Content, ct);

        PreAuthorizationDocumentDto result;
        try
        {
        var document = preAuthorization.AddDocument(type, command.FileName, command.ContentType, storedPath, command.SizeInBytes);

        preAuthorizationRepository.Update(preAuthorization);
        await unitOfWork.SaveChangesAsync(ct);
        result = document.ToDto();
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            storage.Delete(storedPath);
            throw;
        }
        // Keep the file if commit fails: a network failure may leave commit outcome unknown.
        await transaction.CommitAsync(ct);
        return Result.Success(result);
    }
}