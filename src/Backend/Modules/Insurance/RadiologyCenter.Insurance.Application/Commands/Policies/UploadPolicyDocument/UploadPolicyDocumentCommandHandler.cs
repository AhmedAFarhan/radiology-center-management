using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Application.DTOs;

namespace RadiologyCenter.Insurance.Application.Commands.Policies.UploadPolicyDocument;

public static class UploadPolicyDocumentCommandHandler
{
    public static async Task<Result<PolicyDocumentDto>> HandleAsync(
        UploadPolicyDocumentCommand command,
        IInsurancePolicyRepository policyRepository,
        IInsuranceDocumentStorage storage,
        IInsuranceUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var policy = await policyRepository.GetByIdAsync(command.PolicyId, ct);
        if (policy is null)
            return Result.Failure<PolicyDocumentDto>(Error.NotFound("Policy", command.PolicyId));

        var type = DocumentType.FromName<DocumentType>(command.Type);
        if (type is null)
            return Result.Failure<PolicyDocumentDto>(Error.Validation("Type", $"'{command.Type}' is not a valid document type."));

        var relativeDirectory = Path.Combine("policies", command.PolicyId.ToString());
        var storedPath = await storage.SaveAsync(relativeDirectory, command.FileName, command.Content, ct);

        var document = policy.AddDocument(type, command.FileName, command.ContentType, storedPath, command.SizeInBytes);

        policyRepository.Update(policy);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(document.ToDto());
    }
}