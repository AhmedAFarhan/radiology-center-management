using RadiologyCenter.Examinations.Application.Abstractions;

namespace RadiologyCenter.Examinations.Application.Commands.ActivateExaminationType;

public static class ActivateExaminationTypeCommandHandler
{
    public static async Task<Result> HandleAsync(
        ActivateExaminationTypeCommand command,
        IExaminationTypeRepository examinationTypeRepository,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var examinationType = await examinationTypeRepository.GetByIdAsync(command.ExaminationTypeId, ct);
        if (examinationType is null)
            return Result.Failure(Error.NotFound("ExaminationType", command.ExaminationTypeId));

        examinationType.Activate();
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
