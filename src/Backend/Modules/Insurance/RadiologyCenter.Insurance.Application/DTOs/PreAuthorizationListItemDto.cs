namespace RadiologyCenter.Insurance.Application.DTOs;

public sealed record PreAuthorizationListItemDto(
    Guid Id,
    Guid ExaminationId,
    string ExaminationTypeName,
    Guid PatientId,
    string PatientName,
    Guid PolicyId,
    string PolicyNumber,
    decimal EstimatedAmount,
    string Status,
    DateTime RequestedAt,
    DateTime? DecidedAt,
    decimal? ApprovedAmount,
    string? RejectionReason,
    bool IsGovernment);