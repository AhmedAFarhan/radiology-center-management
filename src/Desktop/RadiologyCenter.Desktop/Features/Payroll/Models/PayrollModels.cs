namespace RadiologyCenter.Desktop.Features.Payroll.Models;

public sealed record PayRunDto(
    string Id,
    DateTime RunFrom,
    DateTime RunTo,
    string Status,
    string? ProcessedBy,
    DateTime? ProcessedAt,
    string? Notes,
    IReadOnlyList<PayslipDto> Payslips,
    IReadOnlyList<ReferralFeeStatementDto>? ReferralFeeStatements = null,
    string StatusKey = "");

public sealed record PayRunListItemDto(
    string Id,
    DateTime RunFrom,
    DateTime RunTo,
    string Status,
    string? ProcessedBy,
    DateTime? ProcessedAt,
    string? Notes,
    int EmployeeCount,
    decimal TotalNetPay,
    string StatusKey = "");

public sealed record PayslipDto(
    string Id,
    string PayRunId,
    string StaffId,
    decimal GrossSalary,
    int UnpaidLeaveDays,
    decimal UnpaidLeaveDeduction,
    decimal TotalEarnings,
    decimal TotalDeductions,
    decimal NetSalary,
    string? Notes,
    IReadOnlyList<PayslipComponentDto> Components);

public sealed record PayslipComponentDto(
    string Id,
    string Name,
    decimal Amount,
    bool IsDeduction);

public sealed record ReferralFeeStatementDto(
    string Id,
    string PayRunId,
    string ReferralDoctorId,
    decimal TotalFee,
    int ExamCount);

public sealed class CreatePayRunInput
{
    public DateTime RunFrom { get; set; }
    public DateTime RunTo { get; set; }
    public string? Notes { get; set; }
}

public sealed record SalaryDto(
    string Id,
    string StaffId,
    decimal BaseSalary,
    string SalaryType,
    DateTime EffectiveDate,
    bool IsActive,
    string SalaryTypeKey = "");

public sealed class SalaryInput
{
    public string StaffId { get; set; } = string.Empty;
    public decimal BaseSalary { get; set; }
    public string SalaryType { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }
}

public sealed record SalaryComponentDto(
    string Id,
    string Name,
    string Kind,
    string? Frequency,
    bool IsPercentage,
    bool IsPerWorkDay,
    decimal DefaultValue,
    bool IsActive,
    string KindKey = "",
    string? FrequencyKey = null);

public sealed class SalaryComponentInput
{
    public string Name { get; set; } = string.Empty;
    public string Kind { get; set; } = string.Empty;
    public string? Frequency { get; set; }
    public bool IsPercentage { get; set; }
    public bool IsPerWorkDay { get; set; }
    public decimal DefaultValue { get; set; }
}

public sealed record AllowanceAssignmentDto(
    string Id,
    string StaffId,
    string? SalaryComponentId,
    string Name,
    decimal Amount,
    string? Frequency,
    bool IsPerWorkDay,
    DateTime EffectiveDate,
    DateTime? EndDate,
    bool IsActive,
    string? FrequencyKey = null);

public sealed class AllowanceAssignmentInput
{
    public string StaffId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? SalaryComponentId { get; set; }
    public string? Frequency { get; set; }
    public bool IsPerWorkDay { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public sealed record ExaminationFeeDto(
    string Id,
    string ExaminationTypeId,
    string Role,
    decimal Amount,
    bool IsPercentage,
    bool IsActive,
    string RoleKey = "");

public sealed class ExaminationFeeInput
{
    public string ExaminationTypeId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool IsPercentage { get; set; }
}

public sealed record ReferralFeeDto(
    string Id,
    string ReferralDoctorId,
    string ExaminationTypeId,
    decimal Amount,
    bool IsPercentage,
    bool IsActive);

public sealed class ReferralFeeInput
{
    public string ReferralDoctorId { get; set; } = string.Empty;
    public string ExaminationTypeId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool IsPercentage { get; set; }
}