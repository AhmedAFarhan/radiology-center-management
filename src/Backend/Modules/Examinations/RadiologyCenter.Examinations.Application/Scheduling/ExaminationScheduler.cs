using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Domain.Entities;
using RadiologyCenter.Examinations.Application.Localization;

namespace RadiologyCenter.Examinations.Application.Scheduling;

public sealed class ExaminationScheduler(IExaminationRepository repository, IBookingResourceDirectory directory, ITransactionLock transactionLock)
{
    public async Task<Result> ScheduleAsync(Examination examination, ExaminationTypeInfo type, DateTime requestedTime,
        IUnitOfWorkTransaction transaction, CancellationToken ct)
    {
        var utc = ClinicClock.ToUtc(requestedTime);
        if (utc < DateTime.UtcNow.AddMinutes(-1))
            return Result.Failure(Error.Conflict(ErrorCodes.ScheduledTimePast, "Scheduled time cannot be in the past."));
        var duration = type.StandardDurationMinutes > 0 ? type.StandardDurationMinutes : 30;
        var end = utc.AddMinutes(duration);
        var validation = await ValidateReservationAsync(examination, type, utc, end, transaction, ct);
        if (validation.IsFailure) return validation;
        examination.Schedule(utc, duration);
        return Result.Success();
    }

    public async Task<Result> ValidateReservationAsync(Examination examination, ExaminationTypeInfo type,
        DateTime? startUtc, DateTime? endUtc, IUnitOfWorkTransaction transaction, CancellationToken ct)
    {
        // A single stable lock covers inserts, reschedules and reassignment.
        // The coarse reservation lock also prevents cross-resource lock-order inversions.
        await transactionLock.AcquireAsync(transaction, ["examinations:scheduling"], ct);
        var validation = await directory.ValidateAsync(examination, type, startUtc, endUtc, ct);
        if (validation.IsFailure || !startUtc.HasValue || !endUtc.HasValue) return validation;
        var (conflict, resource) = await ExaminationOverlapChecker.FindConflictAsync(repository, examination.EquipmentId,
            examination.RadiologistId, startUtc.Value, endUtc.Value, examination.Id, ct);
        if (!conflict && examination.TechnicianId.HasValue)
        {
            var existing = await repository.GetScheduledInRangeAsync(startUtc.Value, endUtc.Value, examination.Id, ct);
            if (existing.Any(e => e.TechnicianId == examination.TechnicianId))
                return Result.Failure(Error.Conflict("Examinations.TechnicianOverlap", "The technician is already booked during this interval."));
        }
        return conflict
            ? Result.Failure(Error.Conflict(resource == "equipment" ? ErrorCodes.EquipmentOverlap : ErrorCodes.RadiologistOverlap, $"The {resource} is already booked during this interval."))
            : Result.Success();
    }
}
