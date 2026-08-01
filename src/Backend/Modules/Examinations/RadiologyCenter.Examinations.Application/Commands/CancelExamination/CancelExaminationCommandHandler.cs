using RadiologyCenter.Examinations.Application.Abstractions;

namespace RadiologyCenter.Examinations.Application.Commands.CancelExamination;

public static class CancelExaminationCommandHandler
{
    public static async Task<Result> HandleAsync(
        CancelExaminationCommand command,
        IExaminationRepository examinationRepository,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var examination = await examinationRepository.GetByIdAsync(command.ExaminationId, ct);
        if (examination is null)
            return Result.Failure(Error.NotFound("Examination", command.ExaminationId));

        examination.Cancel(command.Reason);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
