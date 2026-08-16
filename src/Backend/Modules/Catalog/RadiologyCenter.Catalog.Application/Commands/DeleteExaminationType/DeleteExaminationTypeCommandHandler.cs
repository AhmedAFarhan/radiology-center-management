using RadiologyCenter.Catalog.Application.Localization;
using RadiologyCenter.Catalog.Application.Abstractions;

namespace RadiologyCenter.Catalog.Application.Commands.DeleteExaminationType;

public static class DeleteExaminationTypeCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeleteExaminationTypeCommand command,
        IExaminationTypeRepository examinationTypeRepository,
        IExaminationTypeUsageChecker usageChecker,
        ICatalogUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var examinationType = await examinationTypeRepository.GetByIdAsync(command.ExaminationTypeId, ct);
        if (examinationType is null)
            return Result.Failure(Error.NotFound(ErrorCodes.ExaminationTypeNotFound, "ExaminationType", command.ExaminationTypeId));

        if (await usageChecker.HasActiveExaminationsAsync(command.ExaminationTypeId, ct))
            return Result.Failure(Error.Conflict(ErrorCodes.ExaminationTypeInUse, $"Examination type '{examinationType.Name}' cannot be deleted because it is still referenced by active examinations."));

        examinationTypeRepository.Remove(examinationType);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
