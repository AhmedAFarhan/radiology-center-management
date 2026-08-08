namespace RadiologyCenter.Insurance.Application.DTOs;

public sealed record SettlementDto(
    Guid Id,
    decimal Amount,
    string Method,
    DateTime SettledAt,
    string? Reference);