namespace RadiologyCenter.Insurance.Application.DTOs;

public sealed record PolicyDocumentDto(
    Guid Id,
    Guid PolicyId,
    string Type,
    string FileName,
    string ContentType,
    long SizeInBytes,
    DateTime UploadedAt);