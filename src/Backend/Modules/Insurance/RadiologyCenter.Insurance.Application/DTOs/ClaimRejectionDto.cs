namespace RadiologyCenter.Insurance.Application.DTOs;

public sealed record ClaimRejectionDto(
    Guid Id,
    string Code,
    string Reason,
    DateTime RejectedAt);