namespace RadiologyCenter.Payroll.Application.Abstractions;

public interface IExamFeeIncomeResolver
{
    Task<decimal> GetFeeIncomeAsync(Guid staffId, DateTime from, DateTime to, CancellationToken ct = default);
}