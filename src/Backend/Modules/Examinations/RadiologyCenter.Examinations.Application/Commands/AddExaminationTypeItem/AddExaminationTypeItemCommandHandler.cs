using Mapster;
using RadiologyCenter.Examinations.Application.Localization;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.DTOs;

namespace RadiologyCenter.Examinations.Application.Commands.AddExaminationTypeItem;

public static class AddExaminationTypeItemCommandHandler
{
    public static async Task<Result<ExaminationTypeItemDto>> HandleAsync(
        AddExaminationTypeItemCommand command,
        IExaminationTypeDirectory typeDirectory,
        IExaminationTypeItemRepository itemRepository,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var type = await typeDirectory.GetByIdAsync(command.ExaminationTypeId, ct);
        if (type is null)
            return Result.Failure<ExaminationTypeItemDto>(Error.NotFound(ErrorCodes.ExaminationTypeNotFound, "ExaminationType", command.ExaminationTypeId));

        if (await itemRepository.ExistsByItemAsync(command.ExaminationTypeId, command.ItemId, ct))
            return Result.Failure<ExaminationTypeItemDto>(Error.Conflict(ErrorCodes.ItemAlreadyInPreferences, $"This item is already in the preferences for examination type '{type.Code}'."));

        var item = ExaminationTypeItem.Create(
            command.ExaminationTypeId,
            command.ItemId,
            command.Quantity,
            command.IsContrast,
            command.IsRequired,
            command.Notes);

        await itemRepository.AddAsync(item, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(item.Adapt<ExaminationTypeItemDto>());
    }
}