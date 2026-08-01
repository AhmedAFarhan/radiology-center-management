using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Application.Commands.UpdateExaminationType;

public static class UpdateExaminationTypeCommandHandler
{
    public static async Task<Result> HandleAsync(
        UpdateExaminationTypeCommand command,
        IExaminationTypeRepository examinationTypeRepository,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var examinationType = await examinationTypeRepository.GetWithItemsAsync(command.ExaminationTypeId, ct);
        if (examinationType is null)
            return Result.Failure(Error.NotFound("ExaminationType", command.ExaminationTypeId));

        if (await examinationTypeRepository.ExistsByCodeAsync(command.Code, command.ExaminationTypeId, ct))
            return Result.Failure(Error.Validation("ExaminationTypeCodeExists", $"An examination type with code '{command.Code}' already exists."));

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
        {
            var reconcileResult = ReconcileItems(examinationType, command.Items);
            if (reconcileResult is not null)
                return reconcileResult;
        }

        examinationTypeRepository.Update(examinationType);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static Result? ReconcileItems(
        ExaminationType examinationType,
        IReadOnlyList<UpdateExaminationTypeItemRequest> requested)
    {
        var currentItems = examinationType.Items.ToList();
        var requestedIds = requested
            .Where(i => i.ExaminationTypeItemId is not null)
            .Select(i => i.ExaminationTypeItemId!.Value)
            .ToHashSet();

        foreach (var item in currentItems.Where(i => !requestedIds.Contains(i.Id)))
            examinationType.RemoveItem(item.Id);

        foreach (var request in requested)
        {
            if (request.ExaminationTypeItemId is not null)
            {
                if (currentItems.All(i => i.Id != request.ExaminationTypeItemId.Value))
                    return Result.Failure(Error.Validation("ExaminationTypeItemNotFound", $"Preference item '{request.ExaminationTypeItemId.Value}' is not on examination type '{examinationType.Code}'."));

                examinationType.UpdateItem(
                    request.ExaminationTypeItemId.Value,
                    request.ItemId,
                    request.Quantity,
                    request.IsContrast,
                    request.IsRequired,
                    request.Notes);
            }
            else
            {
                examinationType.AddItem(
                    request.ItemId,
                    request.Quantity,
                    request.IsContrast,
                    request.IsRequired,
                    request.Notes);
            }
        }

        return null;
    }
}
