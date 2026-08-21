using RadiologyCenter.Examinations.Application.Localization;
using RadiologyCenter.Examinations.Application.Abstractions;

namespace RadiologyCenter.Examinations.Application.Commands.CompleteExamination;

public static class CompleteExaminationCommandHandler
{
    public static async Task<Result> HandleAsync(
        CompleteExaminationCommand command,
        IExaminationRepository examinationRepository,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var examination = await examinationRepository.GetByIdAsync(command.ExaminationId, ct);
        if (examination is null)
            return Result.Failure(Error.NotFound(ErrorCodes.ExaminationNotFound, "Examination", command.ExaminationId));

        if (examination.RadiologistId is null || examination.TechnicianId is null)
            return Result.Failure(Error.Conflict(
                ErrorCodes.StaffNotAssigned,
                "A radiologist and a technician must be assigned before the examination can be completed."));

        examination.Complete();

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
