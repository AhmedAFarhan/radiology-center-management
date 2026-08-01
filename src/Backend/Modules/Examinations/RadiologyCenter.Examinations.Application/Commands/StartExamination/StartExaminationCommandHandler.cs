using RadiologyCenter.Examinations.Application.Abstractions;

namespace RadiologyCenter.Examinations.Application.Commands.StartExamination;

public static class StartExaminationCommandHandler
{
    public static async Task<Result> HandleAsync(
        StartExaminationCommand command,
        IVisitRepository visitRepository,
        IExaminationsUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        CancellationToken ct)
    {
        var visit = await visitRepository.GetWithExaminationsAsync(command.VisitId, ct);
        if (visit is null)
            return Result.Failure(Error.NotFound("Visit", command.VisitId));

        if (!Guid.TryParse(currentUser.Id, out var performedByUserId))
            return Result.Failure(Error.Unauthorized("An authenticated user is required to start an examination."));

        visit.StartExamination(command.ExaminationId, performedByUserId);

        visitRepository.Update(visit);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
