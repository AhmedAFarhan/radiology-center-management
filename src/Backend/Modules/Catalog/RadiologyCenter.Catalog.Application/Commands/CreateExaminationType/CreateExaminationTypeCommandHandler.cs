using Mapster;
using RadiologyCenter.Catalog.Application.Localization;
using RadiologyCenter.Catalog.Application.Abstractions;
using RadiologyCenter.Catalog.Application.DTOs;
using RadiologyCenter.Catalog.Domain.Enumerations;

namespace RadiologyCenter.Catalog.Application.Commands.CreateExaminationType;

public static class CreateExaminationTypeCommandHandler
{
    public static async Task<Result<ExaminationTypeDto>> HandleAsync(
        CreateExaminationTypeCommand command,
        IExaminationTypeRepository examinationTypeRepository,
        ICatalogUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var modality = Modality.FromName<Modality>(command.Modality);

        if (await examinationTypeRepository.ExistsByCodeAsync(command.Code, ct: ct))
            return Result.Failure<ExaminationTypeDto>(Error.Conflict(ErrorCodes.ExaminationTypeCodeExists, $"An examination type with code '{command.Code}' already exists."));

        var examinationType = ExaminationType.Create(
            command.Code,
            command.Name,
            modality,
            command.BodyPart,
            command.StandardDurationMinutes,
            command.Price,
            command.RequiresPreparation,
            command.RequiresConsent);

        await examinationTypeRepository.AddAsync(examinationType, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(examinationType.Adapt<ExaminationTypeDto>());
    }
}