using RadiologyCenter.Examinations.Application.Abstractions;

namespace RadiologyCenter.Examinations.Application.Commands.RemoveExaminationItem;

public static class RemoveExaminationItemCommandHandler
{
    public static async Task<Result> HandleAsync(
        RemoveExaminationItemCommand command,
        IVisitRepository visitRepository,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var visit = await visitRepository.GetByIdAsync(command.VisitId, ct);
        if (visit is null)
            return Result.Failure(Error.NotFound("Visit", command.VisitId));

        visit.RemoveExaminationItem(command.ExaminationId, command.ExaminationItemId);

        visitRepository.Update(visit);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
