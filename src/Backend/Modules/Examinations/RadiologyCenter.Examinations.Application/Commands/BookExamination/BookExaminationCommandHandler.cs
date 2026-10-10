using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Application.Abstractions.Services;
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
        ExaminationScheduler scheduler,
        INumberSequenceGenerator sequence,
        CancellationToken ct)
    {
        var examinationType = await examinationTypeDirectory.GetWithItemsAsync(command.ExaminationTypeId, ct);
        if (examinationType is null)
            return Result.Failure<ExaminationDto>(Error.NotFound(ErrorCodes.ExaminationTypeNotFound, "ExaminationType", command.ExaminationTypeId));

        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);

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

        var scheduled = await scheduler.ScheduleAsync(examination, examinationType, command.ScheduledAt, transaction, ct);
        if (scheduled.IsFailure) return Result.Failure<ExaminationDto>(scheduled.Error!);

        examination.AssignAccession(await sequence.GenerateNextAsync("ExaminationAccession", "EX", 8, transaction.DbTransaction, ct));
        await examinationRepository.AddAsync(examination, ct);
        await unitOfWork.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return Result.Success(examination.ToDto(examinationType.Name));
    }
}
