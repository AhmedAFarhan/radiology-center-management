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
        var examinationType = await examinationTypeRepository.GetWithItemsAsync(command.ExaminationTypeId, ct);
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

        if (command.Items is not null)
            ReconcileItems(examinationType, command.Items);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static void ReconcileItems(
        ExaminationType examinationType,
        IReadOnlyList<UpdateExaminationTypeItemRequest> requested)
    {
        foreach (var item in examinationType.Items.ToList())
            examinationType.RemoveItem(item.Id);

        foreach (var request in requested)
        {
            examinationType.AddItem(
                request.ItemId,
                request.Quantity,
                request.IsContrast,
                request.IsRequired,
                request.Notes);
        }
    }
}
