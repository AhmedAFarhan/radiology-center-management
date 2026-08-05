using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.DTOs;

namespace RadiologyCenter.Examinations.Application.Queries.GetFinancialAnalytics;

public static class GetFinancialAnalyticsQueryHandler
{
    public static async Task<Result<FinancialAnalyticsDto>> HandleAsync(
        GetFinancialAnalyticsQuery query,
        IExaminationRepository examinationRepository,
        IExaminationTypeRepository examinationTypeRepository,
        CancellationToken ct)
    {
        var projections = await examinationRepository.GetFinancialProjectionAsync(query.From, query.To, ct);

        var typeIds = projections.Select(p => p.ExaminationTypeId).Distinct().ToList();
        var types = await examinationTypeRepository.GetWithItemsByIdsAsync(typeIds, ct);
        var typeLookup = types
            .ToDictionary(t => t.Id, t => (Name: t.Name, Modality: t.Modality.ToString()));

        var examCount = projections.Count;
        var totalCollected = projections.Sum(p => p.Paid);
        var totalBilled = projections.Sum(p => Billable(p));
        var totalDiscounts = projections.Sum(p => p.Price - Billable(p));
        var receivables = projections.Sum(p => p.Remaining);
        var avgPerExam = examCount == 0 ? 0m : totalCollected / examCount;

        var byMonth = projections
            .Where(p => p.CompletedAt is not null)
            .GroupBy(p => $"{p.CompletedAt!.Value.Year:0000}-{p.CompletedAt!.Value.Month:00}")
            .OrderBy(g => g.Key)
            .Select(g => new RevenuePointDto(g.Key, g.Sum(p => p.Paid), g.Sum(Billable)))
            .ToList();

        var byModality = projections
            .GroupBy(p => typeLookup.TryGetValue(p.ExaminationTypeId, out var t) ? t.Modality : "Unknown")
            .OrderByDescending(g => g.Sum(p => p.Paid))
            .Select(g => new RevenueByModalityDto(g.Key, g.Sum(p => p.Paid), g.Count()))
            .ToList();

        var now = DateTime.Today;
        var aging = new List<ReceivableBucketDto>();
        var receivableExams = projections.Where(p => p.Remaining > 0).ToList();

        aging.Add(Bucket(receivableExams, "Current (0-30d)", e => e.CompletedAt is null || (now - e.CompletedAt!.Value.Date).Days <= 30));
        aging.Add(Bucket(receivableExams, "31-60d", e => e.CompletedAt is not null && (now - e.CompletedAt.Value.Date).Days is > 30 and <= 60));
        aging.Add(Bucket(receivableExams, "61-90d", e => e.CompletedAt is not null && (now - e.CompletedAt.Value.Date).Days is > 60 and <= 90));
        aging.Add(Bucket(receivableExams, "90d+", e => e.CompletedAt is not null && (now - e.CompletedAt.Value.Date).Days > 90));

        return Result.Success(new FinancialAnalyticsDto(
            examCount,
            totalCollected,
            totalBilled,
            totalDiscounts,
            receivables,
            avgPerExam,
            byMonth,
            byModality,
            aging));
    }

    private static decimal Billable(ExamFinancialProjection p) =>
        p.IsDiscountPercentage
            ? Math.Round(p.Price * (1m - p.Discount / 100m), 2)
            : p.Price - p.Discount;

    private static ReceivableBucketDto Bucket(
        IEnumerable<ExamFinancialProjection> source,
        string name,
        Func<ExamFinancialProjection, bool> predicate)
    {
        var items = source.Where(predicate).ToList();
        return new ReceivableBucketDto(name, items.Sum(p => p.Remaining), items.Count);
    }
}