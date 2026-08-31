using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.DTOs;
using RadiologyCenter.Examinations.Application.Localization;
using RadiologyCenter.Examinations.Application.Scheduling;
using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Application.Commands.ScheduleExamination;

public static class ScheduleExaminationCommandHandler
{
    public static async Task<Result> HandleAsync(
        ScheduleExaminationCommand command,
        IExaminationRepository examinationRepository,
        IExaminationTypeDirectory examinationTypeDirectory,
        IExaminationsUnitOfWork unitOfWork,
        ITimezoneConverter timezone,
        CancellationToken ct)
    {
        var examination = await examinationRepository.GetByIdAsync(command.ExaminationId, ct);
        if (examination is null)
            return Result.Failure(Error.NotFound(ErrorCodes.ExaminationNotFound, "Examination", command.ExaminationId));

        var localNow = timezone.ToLocal(DateTime.UtcNow);
        if (command.ScheduledAt < localNow.AddMinutes(-1))
            return Result.Failure(Error.Conflict(ErrorCodes.ScheduledTimePast, "Scheduled time cannot be in the past."));

        var examinationType = await examinationTypeDirectory.GetByIdAsync(examination.ExaminationTypeId, ct);
        var durationMinutes = examinationType?.StandardDurationMinutes ?? 30;

        var scheduledEnd = command.ScheduledAt.AddMinutes(durationMinutes);

        var (isConflict, resource) = await ExaminationOverlapChecker.FindConflictAsync(
            examinationRepository, examination.EquipmentId, examination.RadiologistId,
            command.ScheduledAt, scheduledEnd, excludeExaminationId: examination.Id, ct);

        if (isConflict)
        {
            var code = resource == "equipment" ? ErrorCodes.EquipmentOverlap : ErrorCodes.RadiologistOverlap;
            var message = resource == "equipment"
                ? "This equipment is already booked for the selected time slot."
                : "This radiologist is already booked for the selected time slot.";
            return Result.Failure(Error.Conflict(code, message));
        }

        examination.Schedule(command.ScheduledAt, durationMinutes);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
