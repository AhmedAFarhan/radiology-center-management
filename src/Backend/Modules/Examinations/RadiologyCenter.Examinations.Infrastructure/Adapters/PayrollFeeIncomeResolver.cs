using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Abstractions;

namespace RadiologyCenter.Examinations.Infrastructure.Adapters;

public class PayrollFeeIncomeResolver : IExamFeeIncomeResolver
{
    private readonly IExaminationRepository _examinationRepository;
    private readonly RadiologyCenter.Examinations.Application.Abstractions.IExaminationTypeDirectory _examinationTypeDirectory;

    public PayrollFeeIncomeResolver(
        IExaminationRepository examinationRepository,
        RadiologyCenter.Examinations.Application.Abstractions.IExaminationTypeDirectory examinationTypeDirectory)
    {
        _examinationRepository = examinationRepository;
        _examinationTypeDirectory = examinationTypeDirectory;
    }

    public async Task<decimal> GetFeeIncomeAsync(Guid staffId, DateTime from, DateTime to, CancellationToken ct = default)
    {
        var breakdown = await GetFeeIncomeBreakdownAsync(staffId, from, to, ct);
        return breakdown.TotalIncome;
    }

    public async Task<ExamFeeBreakdown> GetFeeIncomeBreakdownAsync(Guid staffId, DateTime from, DateTime to, CancellationToken ct = default)
    {
        var examinations = await _examinationRepository.GetCompletedByRangeAsync(from, to, ct);

        var filtered = examinations
            .Where(e => (e.RadiologistId == staffId || e.TechnicianId == staffId)
                     && e.CompletedAt != null)
            .ToList();

        if (filtered.Count == 0)
            return new ExamFeeBreakdown(0, []);

        var typeIds = filtered.Select(e => e.ExaminationTypeId).Distinct().ToList();
        var types = await _examinationTypeDirectory.GetWithItemsByIdsAsync(typeIds, ct);
        var typeLookup = types.ToDictionary(t => t.Id, t => t.Name);

        decimal total = 0;
        var grouped = new Dictionary<string, (int Count, decimal Rate, decimal Total)>();

        foreach (var row in filtered)
        {
            decimal fee = 0;
            if (row.RadiologistId == staffId)
                fee += row.RadiologistFee ?? 0;
            if (row.TechnicianId == staffId)
                fee += row.TechnicianFee ?? 0;

            if (fee <= 0)
                continue;

            total += fee;

            var key = typeLookup.TryGetValue(row.ExaminationTypeId, out var name) ? name : "Unknown";
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
