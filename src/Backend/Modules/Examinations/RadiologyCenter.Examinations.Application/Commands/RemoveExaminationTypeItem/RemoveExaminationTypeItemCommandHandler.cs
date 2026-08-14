using RadiologyCenter.Examinations.Application.Abstractions;

namespace RadiologyCenter.Examinations.Application.Commands.RemoveExaminationTypeItem;

public static class RemoveExaminationTypeItemCommandHandler
{
    public static async Task<Result> HandleAsync(
        RemoveExaminationTypeItemCommand command,
        IExaminationTypeItemRepository itemRepository,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var item = await itemRepository.GetByIdAsync(command.ExaminationTypeItemId, ct);
        if (item is null || item.ExaminationTypeId != command.ExaminationTypeId)
            return Result.Failure(Error.NotFound("ExaminationTypeItem", command.ExaminationTypeItemId));

        itemRepository.Remove(item);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}