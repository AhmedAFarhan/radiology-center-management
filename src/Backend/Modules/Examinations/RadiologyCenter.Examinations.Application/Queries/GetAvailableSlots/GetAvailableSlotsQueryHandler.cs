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
        IBookingResourceDirectory directory,
        CancellationToken ct)
    {
        if (query.IntervalMinutes is < 5 or > 180 || query.EquipmentId == Guid.Empty || query.Date == default)
            return Result.Failure<IReadOnlyList<AvailableSlotDto>>(Error.Validation("Examinations.InvalidSlotInterval", "Choose a valid date, equipment and an interval between 5 and 180 minutes."));
        if (!await directory.IsOperationalEquipmentAsync(query.EquipmentId, ct))
            return Result.Failure<IReadOnlyList<AvailableSlotDto>>(Error.NotFound("Examinations.EquipmentUnavailable", "OperationalEquipment", query.EquipmentId));
        var date = query.Date.Date;
        var startOfDay = Scheduling.ClinicClock.ToUtc(DateTime.SpecifyKind(date.Add(DayStart.ToTimeSpan()), DateTimeKind.Unspecified));
        var endOfDay = Scheduling.ClinicClock.ToUtc(DateTime.SpecifyKind(date.Add(DayEnd.ToTimeSpan()), DateTimeKind.Unspecified));

        var existing = await examinationRepository.GetScheduledInRangeAsync(
            startOfDay, endOfDay, excludeId: null, ct);

        var equipmentExams = existing
            .Where(e => e.EquipmentId == query.EquipmentId
                && e.ScheduledAt.HasValue)
            .ToList();

        var slots = new List<AvailableSlotDto>();
        var current = startOfDay;

        while (current < endOfDay)
        {
            ct.ThrowIfCancellationRequested();
            var slotEnd = current.AddMinutes(query.IntervalMinutes);
            if (slotEnd > endOfDay)
                slotEnd = endOfDay;

            var overlapping = equipmentExams.FirstOrDefault(e =>
                e.ScheduledAt! < slotEnd && current < (e.ScheduledEnd ?? e.ScheduledAt!.Value.AddMinutes(e.TypeStandardDurationMinutes > 0 ? e.TypeStandardDurationMinutes : 30)));

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
