using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Domain.Entities;
using RadiologyCenter.Payroll.Application.Abstractions;

namespace RadiologyCenter.Localhost.Extensions;

public class PayrollFeeIncomeResolver : IExamFeeIncomeResolver
{
    private readonly IExaminationHistoryRepository _examinationHistoryRepository;

    public PayrollFeeIncomeResolver(IExaminationHistoryRepository examinationHistoryRepository)
        => _examinationHistoryRepository = examinationHistoryRepository;

    public async Task<decimal> GetFeeIncomeAsync(Guid staffId, DateTime from, DateTime to, CancellationToken ct = default)
    {
        var breakdown = await GetFeeIncomeBreakdownAsync(staffId, from, to, ct);
        return breakdown.TotalIncome;
    }

    public async Task<ExamFeeBreakdown> GetFeeIncomeBreakdownAsync(Guid staffId, DateTime from, DateTime to, CancellationToken ct = default)
    {
        var spec = new DynamicSpecification<ExaminationHistory>(h => h.CompletedAt != null);
        spec.AddCriteria(h => h.CompletedAt!.Value >= from && h.CompletedAt!.Value <= to);
        spec.AddCriteria(h => h.RadiologistId == staffId || h.TechnicianId == staffId);

        var rows = await _examinationHistoryRepository.FindAsync(spec, ct);

        decimal total = 0;
        var grouped = new Dictionary<string, (int Count, decimal Rate, decimal Total)>();

        foreach (var row in rows)
        {
            decimal fee = 0;
            if (row.RadiologistId == staffId)
                fee += row.RadiologistFee ?? 0;
            if (row.TechnicianId == staffId)
                fee += row.TechnicianFee ?? 0;

            if (fee <= 0)
                continue;

            total += fee;

            var key = row.TypeName;
            if (grouped.TryGetValue(key, out var existing))
            {
                grouped[key] = (existing.Count + 1, existing.Rate, existing.Total + fee);
            }
            else
            {
                grouped[key] = (1, fee, fee);
            }
        }

        var items = grouped
            .OrderByDescending(kvp => kvp.Value.Total)
            .Select(kvp => new ExamFeeBreakdownItem(kvp.Key, kvp.Value.Count, kvp.Value.Rate, kvp.Value.Total))
            .ToList();

        return new ExamFeeBreakdown(total, items);
    }
}
