namespace RadiologyCenter.Insurance.Application.DTOs;

public sealed record InsuranceCompanyDto(
    Guid Id,
    string Name,
    string? TaxId,
    string? Address,
    string? Phone,
    string? Email);