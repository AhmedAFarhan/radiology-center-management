namespace RadiologyCenter.Cash.Application.DTOs;

public sealed record CashSessionDto(
    Guid Id,
    Guid UserId,
    string UserName,
    Guid? WorkShiftId,
    string Status,
    decimal OpeningFloat,
    decimal Balance,
    DateTime OpenedAt,
    DateTime? ClosedAt,
    string? Notes,
    int EntryCount);