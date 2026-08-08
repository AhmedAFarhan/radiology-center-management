namespace RadiologyCenter.Insurance.Application.DTOs;

public sealed record ClaimDto(
    Guid Id,
    Guid ExaminationId,
    Guid PatientId,
    Guid PolicyId,
    Guid PreAuthorizationId,
    decimal BilledAmount,
    decimal PayerShare,
    decimal PatientShare,
    string Status,
    DateTime? SubmittedAt,
    DateTime? ApprovedAt,
    DateTime? PaidAt,
    decimal TotalSettled,
    decimal RemainingOwed,
    IReadOnlyList<SettlementDto> Settlements,
    IReadOnlyList<ClaimRejectionDto> Rejections);