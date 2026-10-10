using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Application.Localization;

namespace RadiologyCenter.Insurance.Application.Commands.Policies.DeletePolicyDocument;

public static class DeletePolicyDocumentCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeletePolicyDocumentCommand command,
        IPolicyDocumentRepository documentRepository,
        IInsuranceDocumentStorage storage,
        IInsuranceUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var document = await documentRepository.GetByIdAsync(command.DocumentId, ct);
        if (document is null || document.PolicyId != command.PolicyId)
            return Result.Failure(Error.NotFound(ErrorCodes.PolicyDocumentNotFound, "PolicyDocument", command.DocumentId));

        documentRepository.Remove(document);
        await unitOfWork.SaveChangesAsync(ct);
        // Retain the file with soft-deleted metadata for the configured backup/retention policy.

        return Result.Success();
    }
}