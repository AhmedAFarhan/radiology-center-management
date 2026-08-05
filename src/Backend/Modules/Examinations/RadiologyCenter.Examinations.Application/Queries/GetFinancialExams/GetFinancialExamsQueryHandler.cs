using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.DTOs;

namespace RadiologyCenter.Examinations.Application.Queries.GetFinancialExams;

public static class GetFinancialExamsQueryHandler
{
    public static async Task<Result<IReadOnlyList<FinancialExamRowDto>>> HandleAsync(
        GetFinancialExamsQuery query,
        IExaminationRepository examinationRepository,
        IExaminationTypeRepository examinationTypeRepository,
        CancellationToken ct)
    {
        var projections = await examinationRepository.GetFinancialProjectionAsync(query.From, query.To, ct);

        var typeIds = projections.Select(p => p.ExaminationTypeId).Distinct().ToList();
        var types = await examinationTypeRepository.GetWithItemsByIdsAsync(typeIds, ct);
        var typeLookup = types.ToDictionary(t => t.Id, t => t.Name);

        var rows = projections
            .OrderByDescending(p => p.CompletedAt)
            .Select(p => new FinancialExamRowDto(
                p.Id,
                typeLookup.TryGetValue(p.ExaminationTypeId, out var name) ? name : string.Empty,
                p.CompletedAt,
                p.IsDiscountPercentage
                    ? Math.Round(p.Price * (1m - p.Discount / 100m), 2)
                    : p.Price - p.Discount,
                p.Discount,
                p.Paid,
                p.Remaining))
            .ToList();

        return Result.Success<IReadOnlyList<FinancialExamRowDto>>(rows);
    }
}