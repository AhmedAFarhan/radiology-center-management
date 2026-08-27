using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.DTOs;

namespace RadiologyCenter.Examinations.Application.Queries.GetAvailableSlots;

public static class GetAvailableSlotsQueryHandler
{
    private static readonly TimeOnly DayStart = new(8, 0);
    private static readonly TimeOnly DayEnd = new(17, 0);

    public static async Task<Result<IReadOnlyList<AvailableSlotDto>>> HandleAsync(
        GetAvailableSlotsQuery query,
        IExaminationRepository examinationRepository,
        CancellationToken ct)
    {
        var date = query.Date.Date;
        var startOfDay = date.Add(DayStart.ToTimeSpan());
        var endOfDay = date.Add(DayEnd.ToTimeSpan());

        var existing = await examinationRepository.GetScheduledInRangeAsync(
            startOfDay, endOfDay, excludeId: null, ct);

        var equipmentExams = existing
            .Where(e => e.EquipmentId == query.EquipmentId
                && e.ScheduledAt.HasValue
                && e.ScheduledEnd.HasValue)
            .ToList();

        var slots = new List<AvailableSlotDto>();
        var current = startOfDay;

        while (current < endOfDay)
        {
            var slotEnd = current.AddMinutes(query.IntervalMinutes);
            if (slotEnd > endOfDay)
                slotEnd = endOfDay;

            var overlapping = equipmentExams.FirstOrDefault(e =>
                e.ScheduledAt! < slotEnd && current < e.ScheduledEnd!);

            slots.Add(new AvailableSlotDto(
                current,
                slotEnd,
                IsAvailable: overlapping is null,
                ExaminationId: overlapping?.Id.ToString(),
                PatientName: null));

            current = slotEnd;
        }

        return Result.Success<IReadOnlyList<AvailableSlotDto>>(slots);
    }
}
