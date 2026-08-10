namespace RadiologyCenter.Cash.Application.DTOs;

public sealed record CashHandoverDto(
    Guid Id,
    Guid CashSessionId,
    decimal ExpectedTotal,
    decimal CountedTotal,
    decimal OverShortAmount,
    DateTime ClosedAt,
    Guid ClosedByUserId,
    string ClosedByName,
    Guid? ApprovedByUserId,
    DateTime? ApprovedAt,
    Guid? ReceivingCashSessionId,
    string? Notes);