namespace RadiologyCenter.Payroll.Application.Abstractions;

public record PayrollPayslipComponent(
    string Name,
    decimal Amount,
    bool IsDeduction);

public record PayrollPayslipDraft(
    Guid StaffId,
    decimal BaseSalary,
    decimal ExaminationFeeIncome,
    int UnpaidLeaveDays,
    decimal UnpaidLeaveDeduction,
    IReadOnlyList<PayrollPayslipComponent> Components);