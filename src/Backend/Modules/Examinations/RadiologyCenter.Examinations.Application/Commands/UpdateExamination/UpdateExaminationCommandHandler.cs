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
            command.Notes);

        if (command.Paid.HasValue && examination.Paid > 0 && command.Paid.Value != examination.Paid)
            return Result.Failure(Error.Conflict(ErrorCodes.PaidAmountImmutable, "Paid amount cannot be modified once a payment has been recorded."));

        if (command.Discount.HasValue || command.IsDiscountPercentage.HasValue || command.Paid.HasValue)
        {
            examination.SetBilling(
                command.Discount ?? examination.Discount,
                command.IsDiscountPercentage ?? examination.IsDiscountPercentage,
                command.Paid);
        }

        if (command.Items is not null)
            ReconcileItems(examination, command.Items);

        var statusError = ApplyStatusTransition(examination, command.Status, command.ScheduledAt);
        if (statusError is not null)
            return statusError;

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static Result? ApplyStatusTransition(Examination examination, string? status, DateTime? scheduledAt)
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
                examination.Schedule(ClinicClock.ToUtc(scheduledAt.Value));
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
            examination.Schedule(ClinicClock.ToUtc(scheduledAt ?? DateTime.UtcNow));
            return null;
        }

        return Result.Failure(Error.Conflict(
            ErrorCodes.InvalidStatusTransition,
            $"Cannot change examination status from '{current}' to '{target}'."));
    }

    private static void ReconcileItems(
        Examination examination,
        IReadOnlyList<UpdateExaminationItemRequest> requested)
    {
        foreach (var item in examination.Items.ToList())
            examination.RemoveItem(item.Id);

        foreach (var request in requested)
            examination.AddItem(
                request.ItemId,
                request.Quantity,
                request.IsContrast,
                request.IsRequired,
                request.Notes);
    }
}
