namespace RadiologyCenter.Payroll.Application.DTOs;

public record PayRunListItemDto(
    Guid Id,
    DateTime RunFrom,
    DateTime RunTo,
    string Status,
    string? ProcessedBy,
    DateTime? ProcessedAt,
    string? Notes,
    int EmployeeCount,
    decimal TotalNetPay,
    string StatusKey = "");
