using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Examinations.Application.Commands.CreateExamination;
using RadiologyCenter.Examinations.Application.Localization;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.Scheduling;
using RadiologyCenter.Examinations.Domain.Entities;
using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Application.Commands.UpdateExamination;

public static class UpdateExaminationCommandHandler
{
    public static async Task<Result> HandleAsync(
        UpdateExaminationCommand command,
        IExaminationTypeDirectory examinationTypeDirectory,
        IExaminationRepository examinationRepository,
        IExaminationsUnitOfWork unitOfWork,
        ITimezoneConverter timezone,
        CancellationToken ct)
    {
        var examination = await examinationRepository.GetWithItemsAsync(command.ExaminationId, ct);
        if (examination is null)
            return Result.Failure(Error.NotFound(ErrorCodes.ExaminationNotFound, "Examination", command.ExaminationId));

        if (command.PatientId.HasValue && command.PatientId.Value != examination.PatientId)
            examination.UpdatePatient(command.PatientId.Value);

        if (command.ExaminationTypeId.HasValue && command.ExaminationTypeId.Value != examination.ExaminationTypeId)
        {
            var newType = await examinationTypeDirectory.GetWithItemsAsync(command.ExaminationTypeId.Value, ct);
            if (newType is null)
                return Result.Failure(Error.NotFound(ErrorCodes.ExaminationTypeNotFound, "ExaminationType", command.ExaminationTypeId.Value));

            examination.ChangeType(newType.Id, newType.Price);
            foreach (var seeded in ExaminationItemSeeding.Build(newType))
                examination.AddItem(seeded.ItemId, seeded.Quantity, seeded.IsContrast, seeded.IsRequired);
        }

        var priority = ExaminationPriority.FromName<ExaminationPriority>(command.Priority);

        examination.Update(
            command.RadiologistId,
            command.TechnicianId,
            command.ClinicalIndication,
            priority,
            command.ReferralDoctorId,
            command.Notes,
            command.EquipmentId);

        if (command.Discount.HasValue || command.IsDiscountPercentage.HasValue || command.Paid.HasValue)
        {
            examination.SetBilling(
                command.Discount ?? examination.Discount,
                command.IsDiscountPercentage ?? examination.IsDiscountPercentage,
                command.Paid);
        }

        if (command.Items is not null)
            ReconcileItems(examination, command.Items);

        var statusError = await ApplyStatusTransitionAsync(examination, command.Status, command.ScheduledAt, examinationRepository, timezone, ct);
        if (statusError is not null)
            return statusError;

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static async Task<Result?> ApplyStatusTransitionAsync(
        Examination examination,
        string? status,
        DateTime? scheduledAt,
        IExaminationRepository examinationRepository,
        ITimezoneConverter timezone,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(status))
            return null;

        var target = status.Trim();
        var current = examination.Status.Name;
        if (string.Equals(current, target, StringComparison.Ordinal))
        {
            if (string.Equals(target, ExaminationStatus.Scheduled.Name, StringComparison.Ordinal)
                && scheduledAt.HasValue)
            {
                var localNow = timezone.ToLocal(DateTime.UtcNow);
                if (scheduledAt.Value < localNow.AddMinutes(-1))
                    return Result.Failure(Error.Conflict(ErrorCodes.ScheduledTimePast, "Scheduled time cannot be in the past."));

                var overlapError = await CheckOverlapForScheduleAsync(examination, scheduledAt.Value, examinationRepository, ct);
                if (overlapError is not null)
                    return overlapError;

                examination.Schedule(scheduledAt.Value);
            }

            return null;
        }

        if (string.Equals(target, ExaminationStatus.CheckedIn.Name, StringComparison.Ordinal)
            && (current == ExaminationStatus.Requested.Name || current == ExaminationStatus.Scheduled.Name))
        {
            examination.CheckIn();
            return null;
        }

        if (string.Equals(target, ExaminationStatus.Scheduled.Name, StringComparison.Ordinal)
            && (current == ExaminationStatus.Requested.Name || current == ExaminationStatus.Scheduled.Name))
        {
            var scheduledLocal = scheduledAt ?? DateTime.UtcNow;
            var localNow = timezone.ToLocal(DateTime.UtcNow);
            if (scheduledLocal < localNow.AddMinutes(-1))
                return Result.Failure(Error.Conflict(ErrorCodes.ScheduledTimePast, "Scheduled time cannot be in the past."));

            var overlapError = await CheckOverlapForScheduleAsync(examination, scheduledLocal, examinationRepository, ct);
            if (overlapError is not null)
                return overlapError;

            examination.Schedule(scheduledLocal);
            return null;
        }

        return Result.Failure(Error.Conflict(
            ErrorCodes.InvalidStatusTransition,
            $"Cannot change examination status from '{current}' to '{target}'."));
    }

    private static async Task<Result?> CheckOverlapForScheduleAsync(
        Examination examination,
        DateTime scheduledAt,
        IExaminationRepository examinationRepository,
        CancellationToken ct)
    {
        var scheduledEnd = scheduledAt.AddMinutes(30);

        var (isConflict, resource) = await ExaminationOverlapChecker.FindConflictAsync(
            examinationRepository,
            examination.EquipmentId,
            examination.RadiologistId,
            scheduledAt,
            scheduledEnd,
            excludeExaminationId: examination.Id,
            ct);

        if (isConflict)
        {
            var errorCode = resource == "equipment" ? ErrorCodes.EquipmentOverlap : ErrorCodes.RadiologistOverlap;
            return Result.Failure(Error.Conflict(
                errorCode,
                $"The {resource} is already booked during the selected time slot."));
        }

        return null;
    }

    private static void ReconcileItems(
        Examination examination,
        IReadOnlyList<UpdateExaminationItemRequest> requested)
    {
        examination.ClearItems();

        foreach (var request in requested)
            examination.AddItem(
                request.ItemId,
                request.Quantity,
                request.IsContrast,
                request.IsRequired,
                request.Notes);
    }
}
