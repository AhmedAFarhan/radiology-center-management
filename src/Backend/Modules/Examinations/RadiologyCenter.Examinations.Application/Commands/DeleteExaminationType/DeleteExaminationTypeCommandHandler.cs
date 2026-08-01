using RadiologyCenter.Examinations.Application.Abstractions;

namespace RadiologyCenter.Examinations.Application.Commands.DeleteExaminationType;

public static class DeleteExaminationTypeCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeleteExaminationTypeCommand command,
        IExaminationTypeRepository examinationTypeRepository,
        IExaminationsUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        CancellationToken ct)
    {
        var examinationType = await examinationTypeRepository.GetByIdAsync(command.ExaminationTypeId, ct);
        if (examinationType is null)
            return Result.Failure(Error.NotFound("ExaminationType", command.ExaminationTypeId));

        examinationType.Delete(currentUser.Id);
        examinationTypeRepository.Update(examinationType);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
