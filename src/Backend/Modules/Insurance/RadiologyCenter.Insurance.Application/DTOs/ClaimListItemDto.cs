namespace RadiologyCenter.Insurance.Application.DTOs;

public sealed record ClaimListItemDto(
    Guid Id,
    Guid ExaminationId,
    string ExaminationTypeName,
    Guid PatientId,
    string PatientName,
    Guid PolicyId,
    string PolicyNumber,
    Guid PreAuthorizationId,
    decimal BilledAmount,
    decimal PayerShare,
    decimal PatientShare,
    string Status,
    DateTime? SubmittedAt,
    DateTime? ApprovedAt,
    DateTime? PaidAt,
    decimal TotalSettled,
    decimal RemainingOwed);