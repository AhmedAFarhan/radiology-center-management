using RadiologyCenter.Examinations.Application.Localization;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.Scheduling;

namespace RadiologyCenter.Examinations.Application.Commands.ScheduleExamination;

public static class ScheduleExaminationCommandHandler
{
    public static async Task<Result> HandleAsync(
        ScheduleExaminationCommand command,
        IExaminationRepository examinationRepository,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var examination = await examinationRepository.GetByIdAsync(command.ExaminationId, ct);
        if (examination is null)
            return Result.Failure(Error.NotFound(ErrorCodes.ExaminationNotFound, "Examination", command.ExaminationId));

        examination.Schedule(ClinicClock.ToUtc(command.ScheduledAt));

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
