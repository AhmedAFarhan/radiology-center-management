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
            return Result.Failure(Error.NotFound("Examination", command.ExaminationId));

        examination.Complete();

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
