namespace RadiologyCenter.Insurance.Application.Commands.Policies.UploadPolicyDocument;

public record UploadPolicyDocumentCommand(
    Guid PolicyId,
    string Type,
    string FileName,
    string ContentType,
    long SizeInBytes,
    Stream Content) : ICommand;