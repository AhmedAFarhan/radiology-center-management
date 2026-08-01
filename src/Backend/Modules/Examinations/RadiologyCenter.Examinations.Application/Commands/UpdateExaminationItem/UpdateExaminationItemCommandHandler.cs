using RadiologyCenter.Examinations.Application.Abstractions;

namespace RadiologyCenter.Examinations.Application.Commands.UpdateExaminationItem;

public static class UpdateExaminationItemCommandHandler
{
    public static async Task<Result> HandleAsync(
        UpdateExaminationItemCommand command,
        IVisitRepository visitRepository,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var visit = await visitRepository.GetByIdAsync(command.VisitId, ct);
        if (visit is null)
            return Result.Failure(Error.NotFound("Visit", command.VisitId));

        visit.UpdateExaminationItem(
            command.ExaminationId,
            command.ExaminationItemId,
            command.Quantity,
            command.IsContrast,
            command.IsRequired,
            command.Notes);

        visitRepository.Update(visit);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
