using RadiologyCenter.Examinations.Application.Abstractions;

namespace RadiologyCenter.Examinations.Application.Commands.RemoveExaminationItem;

public static class RemoveExaminationItemCommandHandler
{
    public static async Task<Result> HandleAsync(
        RemoveExaminationItemCommand command,
        IExaminationRepository examinationRepository,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var examination = await examinationRepository.GetWithItemsAsync(command.ExaminationId, ct);
        if (examination is null)
            return Result.Failure(Error.NotFound("Examination", command.ExaminationId));

        examination.RemoveItem(command.ExaminationItemId);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
