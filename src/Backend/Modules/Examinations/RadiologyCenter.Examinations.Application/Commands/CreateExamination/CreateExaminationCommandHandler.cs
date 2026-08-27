using RadiologyCenter.Examinations.Application.Localization;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.DTOs;
using RadiologyCenter.Examinations.Application.Scheduling;
using RadiologyCenter.Examinations.Domain.Entities;
using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Application.Commands.CreateExamination;

public static class CreateExaminationCommandHandler
{
    public static async Task<Result<ExaminationDto>> HandleAsync(
        CreateExaminationCommand command,
        IExaminationTypeDirectory examinationTypeDirectory,
        IExaminationRepository examinationRepository,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var examinationType = await examinationTypeDirectory.GetWithItemsAsync(command.ExaminationTypeId, ct);
        if (examinationType is null)
            return Result.Failure<ExaminationDto>(Error.NotFound(ErrorCodes.ExaminationTypeNotFound, "ExaminationType", command.ExaminationTypeId));

        var priority = ExaminationPriority.FromName<ExaminationPriority>(command.Priority);

        var examination = Examination.Create(
            command.PatientId,
            examinationType.Id,
            command.RadiologistId,
            command.TechnicianId,
            command.ClinicalIndication,
            priority,
            examinationType.Price,
            command.ReferralDoctorId,
            command.Discount,
            command.IsDiscountPercentage,
            command.Paid,
            command.Notes,
            command.EquipmentId);

        foreach (var seeded in ExaminationItemSeeding.Build(examinationType))
            examination.AddItem(seeded.ItemId, seeded.Quantity, seeded.IsContrast, seeded.IsRequired);

        if (string.Equals(command.Status, ExaminationStatus.CheckedIn.Name, StringComparison.Ordinal))
            examination.CheckIn();
        else
        {
            var scheduledAtUtc = command.ScheduledAt.HasValue
                ? ClinicClock.ToUtc(command.ScheduledAt.Value)
                : DateTime.UtcNow;
            examination.Schedule(scheduledAtUtc);
        }

        await examinationRepository.AddAsync(examination, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(examination.ToDto(examinationType.Name));
    }
}
