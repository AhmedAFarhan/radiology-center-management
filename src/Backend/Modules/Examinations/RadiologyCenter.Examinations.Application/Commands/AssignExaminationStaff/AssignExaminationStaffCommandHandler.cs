using RadiologyCenter.Examinations.Application.Scheduling;
using RadiologyCenter.Examinations.Application.Localization;
using RadiologyCenter.Examinations.Application.Abstractions;

namespace RadiologyCenter.Examinations.Application.Commands.AssignExaminationStaff;

public static class AssignExaminationStaffCommandHandler
{
    public static async Task<Result> HandleAsync(
        AssignExaminationStaffCommand command,
        IExaminationRepository examinationRepository,
        IExaminationsUnitOfWork unitOfWork,
        IExaminationTypeDirectory typeDirectory,
        ExaminationScheduler scheduler,
        CancellationToken ct)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);
        var examination = await examinationRepository.GetByIdAsync(command.ExaminationId, ct);
        if (examination is null)
            return Result.Failure(Error.NotFound(ErrorCodes.ExaminationNotFound, "Examination", command.ExaminationId));

        examination.AssignStaff(command.RadiologistId, command.TechnicianId, command.EquipmentId);

        var type = await typeDirectory.GetByIdAsync(examination.ExaminationTypeId, ct);
        if (type is null) return Result.Failure(Error.NotFound(ErrorCodes.ExaminationTypeNotFound, "ExaminationType", examination.ExaminationTypeId));
        var validated = await scheduler.ValidateReservationAsync(examination, type, examination.ScheduledAt,
            examination.ScheduledEnd ?? examination.ScheduledAt?.AddMinutes(Math.Max(type.StandardDurationMinutes, 30)), transaction, ct);
        if (validated.IsFailure) return validated;
        await unitOfWork.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return Result.Success();
    }
}
