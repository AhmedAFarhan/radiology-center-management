using RadiologyCenter.Examinations.Application.Abstractions;

namespace RadiologyCenter.Examinations.Application.Commands.DeactivateExaminationType;

public static class DeactivateExaminationTypeCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeactivateExaminationTypeCommand command,
        IExaminationTypeRepository examinationTypeRepository,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var examinationType = await examinationTypeRepository.GetByIdAsync(command.ExaminationTypeId, ct);
        if (examinationType is null)
            return Result.Failure(Error.NotFound("ExaminationType", command.ExaminationTypeId));

        examinationType.Deactivate();
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
