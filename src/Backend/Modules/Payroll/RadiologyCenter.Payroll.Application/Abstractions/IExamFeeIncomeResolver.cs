namespace RadiologyCenter.Payroll.Application.Abstractions;

public record ExamFeeBreakdownItem(
    string ExaminationTypeName,
    int Count,
    decimal FeeRate,
    decimal Total);

public record ExamFeeBreakdown(
    decimal TotalIncome,
    IReadOnlyList<ExamFeeBreakdownItem> Items);

public interface IExamFeeIncomeResolver
{
    Task<decimal> GetFeeIncomeAsync(Guid staffId, DateTime from, DateTime to, CancellationToken ct = default);

    Task<ExamFeeBreakdown> GetFeeIncomeBreakdownAsync(Guid staffId, DateTime from, DateTime to, CancellationToken ct = default);
}
