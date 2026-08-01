using RadiologyCenter.Examinations.Application.Abstractions;

namespace RadiologyCenter.Examinations.Application.Commands.StartExamination;

public static class StartExaminationCommandHandler
{
    public static async Task<Result> HandleAsync(
        StartExaminationCommand command,
        IVisitRepository visitRepository,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var visit = await visitRepository.GetByIdAsync(command.VisitId, ct);
        if (visit is null)
            return Result.Failure(Error.NotFound("Visit", command.VisitId));

        visit.StartExamination(command.ExaminationId, command.PerformedByUserId);

        visitRepository.Update(visit);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
