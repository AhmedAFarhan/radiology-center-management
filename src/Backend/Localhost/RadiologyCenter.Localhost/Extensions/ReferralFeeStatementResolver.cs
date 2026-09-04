using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Abstractions;

namespace RadiologyCenter.Localhost.Extensions;

public class ReferralFeeStatementResolver : IReferralFeeStatementResolver
{
    private readonly IExaminationRepository _examinationRepository;
    private readonly RadiologyCenter.Examinations.Application.Abstractions.IExaminationTypeDirectory _examinationTypeDirectory;

    public ReferralFeeStatementResolver(
        IExaminationRepository examinationRepository,
        RadiologyCenter.Examinations.Application.Abstractions.IExaminationTypeDirectory examinationTypeDirectory)
    {
        _examinationRepository = examinationRepository;
        _examinationTypeDirectory = examinationTypeDirectory;
    }

    public async Task<IReadOnlyList<ReferralFeeExamBreakdown>> GetReferralFeeBreakdownsAsync(
        DateTime from,
        DateTime to,
        CancellationToken ct = default)
    {
        var completedExams = await _examinationRepository.GetCompletedByRangeAsync(from, to, ct);

        var filtered = completedExams
            .Where(e => e.ReferralDoctorId.HasValue && e.ReferralFee.HasValue && e.ReferralFee.Value > 0)
            .ToList();

        if (filtered.Count == 0)
            return [];

        var typeIds = filtered.Select(e => e.ExaminationTypeId).Distinct().ToList();
        var types = await _examinationTypeDirectory.GetWithItemsByIdsAsync(typeIds, ct);
        var typeLookup = types.ToDictionary(t => t.Id, t => t.Name);

        return filtered
            .GroupBy(e => e.ReferralDoctorId!.Value)
            .Select(g => BuildBreakdown(g.Key, g.ToList(), typeLookup))
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
        var completedExams = await _examinationRepository.GetCompletedByRangeAsync(from, to, ct);

        var filtered = completedExams
            .Where(e => e.ReferralDoctorId == referralDoctorId
                     && e.ReferralFee.HasValue
                     && e.ReferralFee.Value > 0)
            .ToList();

        if (filtered.Count == 0)
            return null;

        var typeIds = filtered.Select(e => e.ExaminationTypeId).Distinct().ToList();
        var types = await _examinationTypeDirectory.GetWithItemsByIdsAsync(typeIds, ct);
        var typeLookup = types.ToDictionary(t => t.Id, t => t.Name);

        return BuildBreakdown(referralDoctorId, filtered, typeLookup);
    }

    private static ReferralFeeExamBreakdown BuildBreakdown(
        Guid referralDoctorId,
        List<Examinations.Domain.Entities.Examination> exams,
        IReadOnlyDictionary<Guid, string> typeLookup)
    {
        var items = exams
            .GroupBy(e => typeLookup.TryGetValue(e.ExaminationTypeId, out var name) ? name : BrandConstants.UnknownModality)
            .Select(g => new ReferralFeeExamBreakdownItem(
                g.Key,
                g.Count(),
                Math.Round(g.Sum(e => e.ReferralFee!.Value), 2, MidpointRounding.AwayFromZero)))
            .OrderByDescending(i => i.TotalFee)
            .ToList();

        var totalFee = Math.Round(items.Sum(i => i.TotalFee), 2, MidpointRounding.AwayFromZero);
        var examCount = items.Sum(i => i.Count);

        return new ReferralFeeExamBreakdown(referralDoctorId, totalFee, examCount, items);
    }
}
