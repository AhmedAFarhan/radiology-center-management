using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.Commands.CreateExamination;
using RadiologyCenter.Examinations.Application.DTOs;
using RadiologyCenter.Examinations.Application.Localization;
using RadiologyCenter.Examinations.Application.Scheduling;
using RadiologyCenter.Examinations.Domain.Entities;
using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Application.Commands.BookExamination;

public static class BookExaminationCommandHandler
{
    public static async Task<Result<ExaminationDto>> HandleAsync(
        BookExaminationCommand command,
        IExaminationTypeDirectory examinationTypeDirectory,
        IExaminationRepository examinationRepository,
        IItemSnapshotResolver itemSnapshotResolver,
        IExaminationsUnitOfWork unitOfWork,
        ITimezoneConverter timezone,
        CancellationToken ct)
    {
        var examinationType = await examinationTypeDirectory.GetWithItemsAsync(command.ExaminationTypeId, ct);
        if (examinationType is null)
            return Result.Failure<ExaminationDto>(Error.NotFound(ErrorCodes.ExaminationTypeNotFound, "ExaminationType", command.ExaminationTypeId));

        var localNow = timezone.ToLocal(DateTime.UtcNow);
        if (command.ScheduledAt < localNow.AddMinutes(-1))
            return Result.Failure<ExaminationDto>(Error.Conflict(ErrorCodes.ScheduledTimePast, "Scheduled time cannot be in the past."));

        var scheduledEnd = examinationType.StandardDurationMinutes > 0
            ? command.ScheduledAt.AddMinutes(examinationType.StandardDurationMinutes)
            : command.ScheduledAt.AddMinutes(30);

        if (command.EquipmentId.HasValue)
        {
            var (isConflict, resource) = await ExaminationOverlapChecker.FindConflictAsync(
                examinationRepository, command.EquipmentId, command.RadiologistId,
                command.ScheduledAt, scheduledEnd, excludeExaminationId: null, ct);

            if (isConflict)
            {
                var code = resource == "equipment" ? ErrorCodes.EquipmentOverlap : ErrorCodes.RadiologistOverlap;
                var message = resource == "equipment"
                    ? "This equipment is already booked for the selected time slot."
                    : "This radiologist is already booked for the selected time slot.";
                return Result.Failure<ExaminationDto>(Error.Conflict(code, message));
            }
        }

        var priority = ExaminationPriority.FromName<ExaminationPriority>(command.Priority);

        var examination = Examination.Create(
            command.PatientId,
            examinationType.Id,
            command.RadiologistId,
            command.TechnicianId,
            string.IsNullOrWhiteSpace(command.ClinicalIndication) ? "Scheduled from calendar" : command.ClinicalIndication,
            priority,
            examinationType.Price,
            examinationType.Price,
            examinationType.StandardDurationMinutes,
            command.ReferralDoctorId,
            notes: command.Notes,
            equipmentId: command.EquipmentId);

        var seededItems = ExaminationItemSeeding.Build(examinationType);
        var itemIds = seededItems.Select(i => i.ItemId).ToList();
        var itemCosts = await itemSnapshotResolver.ResolveAsync(itemIds, ct);

        foreach (var seeded in seededItems)
        {
            var unitCost = itemCosts.TryGetValue(seeded.ItemId, out var cost) ? cost : 0m;
            examination.AddItem(seeded.ItemId, seeded.Quantity, seeded.IsContrast, seeded.IsRequired, unitCost: unitCost);
        }

        examination.Schedule(command.ScheduledAt, examinationType.StandardDurationMinutes);

        await examinationRepository.AddAsync(examination, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(examination.ToDto(examinationType.Name));
    }
}
