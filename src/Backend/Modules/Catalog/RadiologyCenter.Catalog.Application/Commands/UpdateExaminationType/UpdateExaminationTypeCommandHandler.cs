using RadiologyCenter.Catalog.Application.Abstractions;
using RadiologyCenter.Catalog.Domain.Enumerations;

namespace RadiologyCenter.Catalog.Application.Commands.UpdateExaminationType;

public static class UpdateExaminationTypeCommandHandler
{
    public static async Task<Result> HandleAsync(
        UpdateExaminationTypeCommand command,
        IExaminationTypeRepository examinationTypeRepository,
        ICatalogUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var examinationType = await examinationTypeRepository.GetByIdAsync(command.ExaminationTypeId, ct);
        if (examinationType is null)
            return Result.Failure(Error.NotFound("ExaminationType", command.ExaminationTypeId));

        if (await examinationTypeRepository.ExistsByCodeAsync(command.Code, command.ExaminationTypeId, ct))
            return Result.Failure(Error.Conflict($"An examination type with code '{command.Code}' already exists."));

        var modality = Modality.FromName<Modality>(command.Modality);

        examinationType.Update(
            command.Code,
            command.Name,
            modality,
            command.BodyPart,
            command.StandardDurationMinutes,
            command.Price,
            command.RequiresPreparation,
            command.RequiresConsent);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}