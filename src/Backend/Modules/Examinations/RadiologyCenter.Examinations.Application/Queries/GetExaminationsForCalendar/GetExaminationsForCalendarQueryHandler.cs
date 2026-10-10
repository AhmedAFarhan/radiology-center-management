using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.DTOs;
using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Application.Queries.GetExaminationsForCalendar;

public static class GetExaminationsForCalendarQueryHandler
{
    public static async Task<Result<IReadOnlyList<CalendarSlotDto>>> HandleAsync(
        GetExaminationsForCalendarQuery query,
        IExaminationRepository examinationRepository,
        IExaminationTypeDirectory examinationTypeDirectory,
        IPatientInfoResolver patientInfoResolver,
        CancellationToken ct)
    {
        var startUtc = Scheduling.ClinicClock.ToUtc(query.StartDate);
        var endUtc = Scheduling.ClinicClock.ToUtc(query.EndDate);
        if (endUtc <= startUtc || endUtc - startUtc > TimeSpan.FromDays(31))
            return Result.Failure<IReadOnlyList<CalendarSlotDto>>(Error.Validation("Examinations.InvalidCalendarRange", "Select a calendar range of at most 31 days."));

        var examinations = await examinationRepository.GetScheduledInRangeAsync(
            startUtc, endUtc, excludeId: null, ct);

        if (query.EquipmentId.HasValue)
            examinations = examinations.Where(e => e.EquipmentId == query.EquipmentId.Value).ToList();

        if (query.RadiologistId.HasValue)
            examinations = examinations.Where(e => e.RadiologistId == query.RadiologistId.Value).ToList();

        var typeIds = examinations.Select(e => e.ExaminationTypeId).Distinct().ToList();
        var types = await examinationTypeDirectory.GetWithItemsByIdsAsync(typeIds, ct);
        var typeDict = types.ToDictionary(t => t.Id);

        if (!string.IsNullOrEmpty(query.Modality))
            examinations = examinations.Where(e =>
                typeDict.TryGetValue(e.ExaminationTypeId, out var t)
                && string.Equals(t.Modality, query.Modality, StringComparison.OrdinalIgnoreCase))
                .ToList();

        var patientIds = examinations.Select(e => e.PatientId).Distinct().ToList();
        var patients = new List<PatientInfo?>();
        foreach (var id in patientIds)
            patients.Add(await patientInfoResolver.ResolveAsync(id, ct));
        var patientDict = patients
            .Where(p => p != null)
            .ToDictionary(p => p!.Id, p => p!);

        var result = examinations.Select(e =>
        {
            typeDict.TryGetValue(e.ExaminationTypeId, out var typeInfo);
            patientDict.TryGetValue(e.PatientId, out var patient);

            return new CalendarSlotDto(
                e.Id,
                e.EquipmentId,
                null,
                e.RadiologistId,
                null,
                patient?.FullName ?? "Unknown Patient",
                typeInfo?.Name ?? BrandConstants.UnknownModality,
                typeInfo?.Modality ?? "",
                e.ScheduledAt ?? DateTime.MinValue,
                e.ScheduledEnd ?? e.ScheduledAt!.Value.AddMinutes(e.TypeStandardDurationMinutes > 0 ? e.TypeStandardDurationMinutes : 30),
                e.Status.Name,
                e.Priority.Name);
        }).ToList();

        return Result.Success<IReadOnlyList<CalendarSlotDto>>(result);
    }
}
