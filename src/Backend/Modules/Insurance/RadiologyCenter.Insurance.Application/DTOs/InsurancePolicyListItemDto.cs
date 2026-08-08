namespace RadiologyCenter.Insurance.Application.DTOs;

public sealed record InsurancePolicyListItemDto(
    Guid Id,
    Guid CompanyId,
    string CompanyName,
    Guid PatientId,
    string PatientName,
    string PolicyNumber,
    decimal CoveragePercent,
    DateTime EffectiveFrom,
    DateTime? EffectiveTo,
    string Status,
    bool IsGovernment,
    bool IsActive);