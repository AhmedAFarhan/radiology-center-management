namespace RadiologyCenter.Insurance.Application.DTOs;

public sealed record PreAuthorizationDocumentDto(
    Guid Id,
    Guid PreAuthorizationId,
    string Type,
    string FileName,
    string ContentType,
    long SizeInBytes,
    DateTime UploadedAt);