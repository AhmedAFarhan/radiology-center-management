using RadiologyCenter.Examinations.Application.Localization;
using RadiologyCenter.Examinations.Application.Abstractions;

namespace RadiologyCenter.Examinations.Application.Commands.CheckInExamination;

public static class CheckInExaminationCommandHandler
{
    public static async Task<Result> HandleAsync(
        CheckInExaminationCommand command,
        IExaminationRepository examinationRepository,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var examination = await examinationRepository.GetByIdAsync(command.ExaminationId, ct);
        if (examination is null)
            return Result.Failure(Error.NotFound(ErrorCodes.ExaminationNotFound, "Examination", command.ExaminationId));

        examination.CheckIn();

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
