using RadiologyCenter.Examinations.Application.Abstractions;

namespace RadiologyCenter.Examinations.Application.Commands.CancelExamination;

public static class CancelExaminationCommandHandler
{
    public static async Task<Result> HandleAsync(
        CancelExaminationCommand command,
        IVisitRepository visitRepository,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var visit = await visitRepository.GetByIdAsync(command.VisitId, ct);
        if (visit is null)
            return Result.Failure(Error.NotFound("Visit", command.VisitId));

        visit.CancelExamination(command.ExaminationId, command.Reason);

        visitRepository.Update(visit);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
