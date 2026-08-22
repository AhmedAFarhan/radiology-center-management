namespace RadiologyCenter.Insurance.Application.DTOs;

public sealed record PreAuthorizationDto(
    Guid Id,
    Guid ExaminationId,
    Guid PatientId,
    Guid PolicyId,
    decimal EstimatedAmount,
    string Status,
    DateTime RequestedAt,
    DateTime? DecidedAt,
    decimal? ApprovedAmount,
    string? RejectionReason,
    bool IsGovernment,
    IReadOnlyList<PreAuthorizationDocumentDto>? Documents = null,
    string StatusKey = "");