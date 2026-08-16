using RadiologyCenter.Examinations.Application.Localization;
using RadiologyCenter.Examinations.Application.Abstractions;

namespace RadiologyCenter.Examinations.Application.Commands.StartExamination;

public static class StartExaminationCommandHandler
{
    public static async Task<Result> HandleAsync(
        StartExaminationCommand command,
        IExaminationRepository examinationRepository,
        IExaminationsUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        CancellationToken ct)
    {
        if (!Guid.TryParse(currentUser.Id, out var performedByUserId))
            return Result.Failure(Error.Unauthorized(ErrorCodes.AuthenticationRequired, "An authenticated user is required to start an examination."));

        var examination = await examinationRepository.GetByIdAsync(command.ExaminationId, ct);
        if (examination is null)
            return Result.Failure(Error.NotFound(ErrorCodes.ExaminationNotFound, "Examination", command.ExaminationId));

        examination.Start(performedByUserId);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
