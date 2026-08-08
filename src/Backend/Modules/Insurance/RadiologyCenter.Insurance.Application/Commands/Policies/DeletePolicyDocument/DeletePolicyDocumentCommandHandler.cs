using RadiologyCenter.Insurance.Application.Abstractions;

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
            return Result.Failure(Error.NotFound("PolicyDocument", command.DocumentId));

        storage.Delete(document.StoredPath);

        documentRepository.Remove(document);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}