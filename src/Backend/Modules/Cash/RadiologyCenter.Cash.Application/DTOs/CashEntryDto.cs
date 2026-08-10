namespace RadiologyCenter.Cash.Application.DTOs;

public sealed record CashEntryDto(
    Guid Id,
    Guid CashSessionId,
    string Direction,
    string Reason,
    decimal Amount,
    string? Description,
    string? ReferenceId,
    DateTime OccurredAt);