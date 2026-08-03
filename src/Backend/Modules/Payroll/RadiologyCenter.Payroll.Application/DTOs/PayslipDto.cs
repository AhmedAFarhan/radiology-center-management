namespace RadiologyCenter.Payroll.Application.DTOs;

public record PayslipDto(
    Guid Id,
    Guid PayRunId,
    Guid StaffId,
    decimal GrossSalary,
    int UnpaidLeaveDays,
    decimal UnpaidLeaveDeduction,
    decimal TotalEarnings,
    decimal TotalDeductions,
    decimal NetSalary,
    string? Notes,
    IReadOnlyList<PayslipComponentDto> Components);