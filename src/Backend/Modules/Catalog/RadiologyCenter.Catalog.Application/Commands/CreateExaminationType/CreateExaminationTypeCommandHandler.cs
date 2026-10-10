using RadiologyCenter.BuildingBlocks.Domain.Specifications;
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
        ITransactionLock transactionLock,
        CancellationToken ct)
    {
        var modality = Modality.FromName<Modality>(command.Modality);

        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);
        await transactionLock.AcquireAsync(transaction, ["catalog:examination-types"], ct);
        var duplicate = await examinationTypeRepository.FindSingleAsync(new DynamicSpecification<ExaminationType>(t => t.Name == command.Name.Trim()), ct);
        if (duplicate is not null)
            return Result.Failure<ExaminationTypeDto>(Error.Conflict("Catalog.ExaminationTypeNameExists", "An examination type with this name already exists."));

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
        await unitOfWork.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return Result.Success(examinationType.Adapt<ExaminationTypeDto>());
    }
}