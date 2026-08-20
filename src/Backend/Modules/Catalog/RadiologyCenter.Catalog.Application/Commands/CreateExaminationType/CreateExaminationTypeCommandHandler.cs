using Mapster;
using RadiologyCenter.BuildingBlocks.Application.Abstractions.Services;
using RadiologyCenter.Catalog.Application.Abstractions;
using RadiologyCenter.Catalog.Application.DTOs;
using RadiologyCenter.Catalog.Domain.Enumerations;

namespace RadiologyCenter.Catalog.Application.Commands.CreateExaminationType;

public static class CreateExaminationTypeCommandHandler
{
    public static async Task<Result<ExaminationTypeDto>> HandleAsync(
        CreateExaminationTypeCommand command,
        IExaminationTypeRepository examinationTypeRepository,
        INumberSequenceGenerator numberSequenceGenerator,
        ICatalogUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var modality = Modality.FromName<Modality>(command.Modality);

        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);

        var code = await numberSequenceGenerator.GenerateNextAsync(
            "ExaminationType",
            "EXM",
            4,
            transaction.DbTransaction,
            ct);

        var examinationType = ExaminationType.Create(
            code,
            command.Name,
            modality,
            command.BodyPart,
            command.StandardDurationMinutes,
            command.Price,
            command.RequiresPreparation,
            command.RequiresConsent);

        await examinationTypeRepository.AddAsync(examinationType, ct);
        await transaction.CommitAsync(ct);

        return Result.Success(examinationType.Adapt<ExaminationTypeDto>());
    }
}