using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Abstractions;

namespace RadiologyCenter.Localhost.Extensions;

public class ReferralFeeStatementResolver : IReferralFeeStatementResolver
{
    private readonly IExaminationHistoryRepository _examinationHistoryRepository;

    public ReferralFeeStatementResolver(IExaminationHistoryRepository examinationHistoryRepository)
        => _examinationHistoryRepository = examinationHistoryRepository;

    public async Task<IReadOnlyList<ReferralFeeExamBreakdown>> GetReferralFeeBreakdownsAsync(
        DateTime from,
        DateTime to,
        CancellationToken ct = default)
    {
        var completedExams = await _examinationHistoryRepository.GetByCompletedRangeAsync(from, to, ct);

        return completedExams
            .Where(h => h.ReferralDoctorId.HasValue && h.ReferralFee.HasValue && h.ReferralFee.Value > 0)
            .GroupBy(h => h.ReferralDoctorId!.Value)
            .Select(g => BuildBreakdown(g.Key, g.ToList()))
            .Where(b => b.TotalFee > 0)
            .OrderByDescending(b => b.TotalFee)
            .ToList();
    }

    public async Task<ReferralFeeExamBreakdown?> GetReferralFeeBreakdownAsync(
        Guid referralDoctorId,
        DateTime from,
        DateTime to,
        CancellationToken ct = default)
    {
        var completedExams = await _examinationHistoryRepository.GetByCompletedRangeAsync(from, to, ct);

        var filtered = completedExams
            .Where(h => h.ReferralDoctorId == referralDoctorId
                     && h.ReferralFee.HasValue
                     && h.ReferralFee.Value > 0)
            .ToList();

        if (filtered.Count == 0)
            return null;

        return BuildBreakdown(referralDoctorId, filtered);
    }

    private static ReferralFeeExamBreakdown BuildBreakdown(Guid referralDoctorId, List<Examinations.Domain.Entities.ExaminationHistory> exams)
    {
        var items = exams
            .GroupBy(h => h.TypeName)
            .Select(g => new ReferralFeeExamBreakdownItem(
                g.Key,
                g.Count(),
                Math.Round(g.Sum(h => h.ReferralFee!.Value), 2, MidpointRounding.AwayFromZero)))
            .OrderByDescending(i => i.TotalFee)
            .ToList();

        var totalFee = Math.Round(items.Sum(i => i.TotalFee), 2, MidpointRounding.AwayFromZero);
        var examCount = items.Sum(i => i.Count);

        return new ReferralFeeExamBreakdown(referralDoctorId, totalFee, examCount, items);
    }
}
