using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Domain.Entities;
using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Application.Scheduling;

internal static class ExaminationOverlapChecker
{
    public static async Task<(bool IsConflict, string Resource)> FindConflictAsync(
        IExaminationRepository examinationRepository,
        Guid? equipmentId,
        Guid? radiologistId,
        DateTime scheduledAt,
        DateTime scheduledEnd,
        Guid? excludeExaminationId,
        CancellationToken ct)
    {
        var existing = await examinationRepository.GetScheduledInRangeAsync(
            scheduledAt, scheduledEnd, excludeExaminationId, ct);

        if (equipmentId.HasValue
            && existing.Any(e => e.EquipmentId == equipmentId.Value
                && e.ScheduledAt < scheduledEnd
                && scheduledAt < e.ScheduledEnd))
        {
            return (true, "equipment");
        }

        if (radiologistId.HasValue
            && existing.Any(e => e.RadiologistId == radiologistId.Value
                && e.ScheduledAt < scheduledEnd
                && scheduledAt < e.ScheduledEnd))
        {
            return (true, "radiologist");
        }

        return (false, string.Empty);
    }
}
