using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.DTOs;
using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Application.Queries.GetOperationalAnalytics;

public static class GetOperationalAnalyticsQueryHandler
{
    public static async Task<Result<OperationalAnalyticsDto>> HandleAsync(
        GetOperationalAnalyticsQuery query,
        IExaminationRepository examinationRepository,
        IExaminationTypeDirectory examinationTypeDirectory,
        CancellationToken ct)
    {
        var projections = await examinationRepository.GetOperationalProjectionAsync(query.From, query.To, ct);

        var typeIds = projections.Select(p => p.ExaminationTypeId).Distinct().ToList();
        var types = await examinationTypeDirectory.GetWithItemsByIdsAsync(typeIds, ct);
        var typeLookup = types.ToDictionary(t => t.Id, t => t.Modality);

        var total = projections.Count;
        var completed = projections.Count(p => p.Status.Name == ExaminationStatus.Completed.Name);
        var cancelled = projections.Count(p => p.Status.Name == ExaminationStatus.Cancelled.Name);

        var terminal = completed + cancelled;
        var completionRate = terminal == 0 ? 0m : Math.Round((decimal)completed / terminal, 4);

        var completedExams = projections.Where(p => p.Status.Name == ExaminationStatus.Completed.Name).ToList();
        var durations = completedExams
            .Where(p => p.StartedAt is not null && p.CompletedAt is not null)
            .Select(p => (p.CompletedAt!.Value - p.StartedAt!.Value).TotalMinutes)
            .ToList();
        var avgDuration = durations.Count == 0 ? 0d : durations.Average();

        var started = projections
            .Where(p => p.StartedAt is not null)
            .Select(p => (p.StartedAt!.Value - p.CreatedAt).TotalMinutes)
            .ToList();
        var avgTimeToStart = started.Count == 0 ? 0d : started.Average();

        var funnel = projections
            .GroupBy(p => p.Status.Name)
            .OrderBy(g => g.Key)
            .Select(g => new StatusCountDto(g.Key, g.Count()))
            .ToList();

        var byMonth = projections
            .GroupBy(p => $"{p.CreatedAt.Year:0000}-{p.CreatedAt.Month:00}")
            .OrderBy(g => g.Key)
            .Select(g => new MonthlyVolumeDto(
                g.Key,
                g.Count(),
                g.Count(p => p.Status.Name == ExaminationStatus.Completed.Name)))
            .ToList();

        var byModality = projections
            .GroupBy(p => typeLookup.TryGetValue(p.ExaminationTypeId, out var modality) ? modality : BrandConstants.UnknownModality)
            .OrderByDescending(g => g.Count())
            .Select(g => new ModalityVolumeDto(
                g.Key,
                g.Count(),
                g.Count(p => p.Status.Name == ExaminationStatus.Completed.Name)))
            .ToList();

        var byPriority = projections
            .GroupBy(p => p.Priority.Name)
            .OrderBy(g => g.Key)
            .Select(g => new PriorityVolumeDto(g.Key, g.Count()))
            .ToList();

        return Result.Success(new OperationalAnalyticsDto(
            total,
            completed,
            cancelled,
            completionRate,
            avgDuration,
            avgTimeToStart,
            funnel,
            byMonth,
            byModality,
            byPriority));
    }
}
