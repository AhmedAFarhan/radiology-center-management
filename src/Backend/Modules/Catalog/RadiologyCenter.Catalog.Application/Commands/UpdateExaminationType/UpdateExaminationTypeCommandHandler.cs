using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.Catalog.Application.Localization;
using RadiologyCenter.Catalog.Application.Abstractions;
using RadiologyCenter.Catalog.Domain.Enumerations;

namespace RadiologyCenter.Catalog.Application.Commands.UpdateExaminationType;

public static class UpdateExaminationTypeCommandHandler
{
    public static async Task<Result> HandleAsync(
        UpdateExaminationTypeCommand command,
        IExaminationTypeRepository examinationTypeRepository,
        ICatalogUnitOfWork unitOfWork,
        ITransactionLock transactionLock,
        CancellationToken ct)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);
        await transactionLock.AcquireAsync(transaction, ["catalog:examination-types"], ct);
        var examinationType = await examinationTypeRepository.GetByIdAsync(command.ExaminationTypeId, ct);
        if (examinationType is null)
            return Result.Failure(Error.NotFound(ErrorCodes.ExaminationTypeNotFound, "ExaminationType", command.ExaminationTypeId));

        var duplicate = await examinationTypeRepository.FindSingleAsync(new DynamicSpecification<ExaminationType>(t => t.Id != command.ExaminationTypeId && t.Name == command.Name.Trim()), ct);
        if (duplicate is not null)
            return Result.Failure(Error.Conflict("Catalog.ExaminationTypeNameExists", "An examination type with this name already exists."));

        var modality = Modality.FromName<Modality>(command.Modality);

        examinationType.Update(
            command.Name,
            modality,
            command.BodyPart,
            command.StandardDurationMinutes,
            command.Price,
            command.RequiresPreparation,
            command.RequiresConsent);

        await unitOfWork.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return Result.Success();
    }
}