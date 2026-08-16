using RadiologyCenter.Catalog.Application.Localization;
using RadiologyCenter.Catalog.Application.Abstractions;

namespace RadiologyCenter.Catalog.Application.Commands.DeactivateExaminationType;

public static class DeactivateExaminationTypeCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeactivateExaminationTypeCommand command,
        IExaminationTypeRepository examinationTypeRepository,
        ICatalogUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var examinationType = await examinationTypeRepository.GetByIdAsync(command.ExaminationTypeId, ct);
        if (examinationType is null)
            return Result.Failure(Error.NotFound(ErrorCodes.ExaminationTypeNotFound, "ExaminationType", command.ExaminationTypeId));

        examinationType.Deactivate();
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
