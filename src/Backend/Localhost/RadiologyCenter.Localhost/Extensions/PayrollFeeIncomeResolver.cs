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
        var spec = new DynamicSpecification<ExaminationHistory>(h => h.CompletedAt != null);
        spec.AddCriteria(h => h.CompletedAt!.Value >= from && h.CompletedAt!.Value <= to);
        spec.AddCriteria(h => h.RadiologistId == staffId || h.TechnicianId == staffId);

        var rows = await _examinationHistoryRepository.FindAsync(spec, ct);

        decimal total = 0;
        foreach (var row in rows)
        {
            if (row.RadiologistId == staffId)
                total += row.RadiologistFee ?? 0;
            if (row.TechnicianId == staffId)
                total += row.TechnicianFee ?? 0;
        }

        return total;
    }
}