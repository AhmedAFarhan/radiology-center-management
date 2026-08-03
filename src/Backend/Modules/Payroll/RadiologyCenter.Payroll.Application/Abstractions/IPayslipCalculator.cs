namespace RadiologyCenter.Payroll.Application.Abstractions;

public interface IPayslipCalculator
{
    Task<PayrollPayslipDraft?> CalculateAsync(Guid staffId, DateTime from, DateTime to, CancellationToken ct = default);
}