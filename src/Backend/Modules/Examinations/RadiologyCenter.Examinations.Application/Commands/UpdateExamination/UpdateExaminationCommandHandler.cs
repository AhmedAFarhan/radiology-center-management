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
        ExaminationScheduler scheduler,
        CancellationToken ct)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);
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

            examination.ChangeType(newType.Id, newType.Price, newType.StandardDurationMinutes);
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

        if (command.Paid.HasValue && command.Paid.Value != examination.Paid)
            return Result.Failure(Error.Validation("Examinations.UsePaymentCommand", "Record payments or refunds through the payment workflow."));

        if (command.Discount.HasValue || command.IsDiscountPercentage.HasValue || command.Paid.HasValue)
        {
            examination.SetBilling(
                command.Discount ?? examination.Discount,
                command.IsDiscountPercentage ?? examination.IsDiscountPercentage,
                command.Paid);
        }

        if (command.Items is not null)
            ReconcileItems(examination, command.Items);

        var type = await examinationTypeDirectory.GetByIdAsync(examination.ExaminationTypeId, ct);
        if (type is null) return Result.Failure(Error.NotFound(ErrorCodes.ExaminationTypeNotFound, "ExaminationType", examination.ExaminationTypeId));
        var target = string.IsNullOrWhiteSpace(command.Status) ? examination.Status.Name : command.Status.Trim();
        if (target == ExaminationStatus.Scheduled.Name && examination.Status != ExaminationStatus.Scheduled && examination.Status != ExaminationStatus.Requested)
            return Result.Failure(Error.Conflict(ErrorCodes.InvalidStatusTransition, "This examination cannot be scheduled."));
        if (target == ExaminationStatus.Scheduled.Name)
        {
            var scheduled = await scheduler.ScheduleAsync(examination, type, command.ScheduledAt ?? examination.ScheduledAt ?? DateTime.UtcNow, transaction, ct);
            if (scheduled.IsFailure) return scheduled;
        }
        else
        {
            var validated = await scheduler.ValidateReservationAsync(examination, type, examination.ScheduledAt, examination.ScheduledEnd, transaction, ct);
            if (validated.IsFailure) return validated;
            if (target == ExaminationStatus.CheckedIn.Name && (examination.Status == ExaminationStatus.Requested || examination.Status == ExaminationStatus.Scheduled))
                examination.CheckIn();
            else if (target != examination.Status.Name)
                return Result.Failure(Error.Conflict(ErrorCodes.InvalidStatusTransition, "Use the dedicated examination lifecycle action."));
        }
        await unitOfWork.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return Result.Success();
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
