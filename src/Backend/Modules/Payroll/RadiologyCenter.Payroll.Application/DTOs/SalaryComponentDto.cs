namespace RadiologyCenter.Payroll.Application.DTOs;

public record SalaryComponentDto(
    Guid Id,
    string Name,
    string Kind,
    string? Frequency,
    bool IsPercentage,
    bool IsPerWorkDay,
    decimal DefaultValue,
    bool IsActive,
    string KindKey = "",
    string? FrequencyKey = null);