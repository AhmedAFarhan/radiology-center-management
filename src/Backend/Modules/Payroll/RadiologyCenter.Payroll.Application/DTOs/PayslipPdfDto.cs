namespace RadiologyCenter.Payroll.Application.DTOs;

public record PayslipPdfDto
{
    public string StaffFullName { get; init; } = string.Empty;
    public string StaffPosition { get; init; } = string.Empty;
    public string? StaffDepartment { get; init; }
    public string? StaffSpecialization { get; init; }
    public string StaffPhoneNumber { get; init; } = string.Empty;
    public DateTime StaffHireDate { get; init; }

    public DateTime RunFrom { get; init; }
    public DateTime RunTo { get; init; }
    public string PayRunStatus { get; init; } = string.Empty;

    public decimal GrossSalary { get; init; }
    public int UnpaidLeaveDays { get; init; }
    public decimal UnpaidLeaveDeduction { get; init; }

    public IReadOnlyList<PayslipComponentDto> Components { get; init; } = [];

    public decimal TotalEarnings { get; init; }
    public decimal TotalDeductions { get; init; }
    public decimal NetSalary { get; init; }

    public IReadOnlyList<ExamFeeBreakdownItemDto> ExaminationFeeBreakdown { get; init; } = [];
    public decimal ExaminationFeeTotal { get; init; }
}

public record ExamFeeBreakdownItemDto(
    string ExaminationTypeName,
    int Count,
    decimal FeeRate,
    decimal Total);
