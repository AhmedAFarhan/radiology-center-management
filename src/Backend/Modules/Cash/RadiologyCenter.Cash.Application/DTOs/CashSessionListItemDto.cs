namespace RadiologyCenter.Cash.Application.DTOs;

public sealed record CashSessionListItemDto(
    Guid Id,
    Guid UserId,
    string UserName,
    string Status,
    decimal OpeningFloat,
    decimal Balance,
    DateTime OpenedAt,
    DateTime? ClosedAt,
    int EntryCount,
    string StatusKey = "");
