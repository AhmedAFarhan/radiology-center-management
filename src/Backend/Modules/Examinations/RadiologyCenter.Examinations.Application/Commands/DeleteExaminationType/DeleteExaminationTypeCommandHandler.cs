using RadiologyCenter.Examinations.Application.Abstractions;

namespace RadiologyCenter.Examinations.Application.Commands.DeleteExaminationType;

public static class DeleteExaminationTypeCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeleteExaminationTypeCommand command,
        IExaminationTypeRepository examinationTypeRepository,
        IExaminationRepository examinationRepository,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var examinationType = await examinationTypeRepository.GetByIdAsync(command.ExaminationTypeId, ct);
        if (examinationType is null)
            return Result.Failure(Error.NotFound("ExaminationType", command.ExaminationTypeId));

        if (await examinationRepository.HasActiveExaminationsByTypeAsync(command.ExaminationTypeId, ct))
            return Result.Failure(Error.Conflict(
                $"Examination type '{examinationType.Name}' cannot be deleted because it is still referenced by active examinations."));

        examinationTypeRepository.Remove(examinationType);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
