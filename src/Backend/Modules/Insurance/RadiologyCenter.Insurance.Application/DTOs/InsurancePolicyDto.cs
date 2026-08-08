namespace RadiologyCenter.Insurance.Application.DTOs;

public sealed record InsurancePolicyDto(
    Guid Id,
    Guid CompanyId,
    Guid PatientId,
    string PolicyNumber,
    decimal CoveragePercent,
    decimal Deductible,
    decimal Copay,
    DateTime EffectiveFrom,
    DateTime? EffectiveTo,
    string Status);