namespace RadiologyCenter.Insurance.Application.Commands.Policies.DeletePolicyDocument;

public record DeletePolicyDocumentCommand(
    Guid PolicyId,
    Guid DocumentId) : ICommand;