using RadiologyCenter.Catalog.Application.Abstractions;

namespace RadiologyCenter.Catalog.Application.Commands.RemoveExaminationTypeItem;

public static class RemoveExaminationTypeItemCommandHandler
{
    public static async Task<Result> HandleAsync(
        RemoveExaminationTypeItemCommand command,
        IExaminationTypeRepository examinationTypeRepository,
        ICatalogUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var examinationType = await examinationTypeRepository.GetWithItemsAsync(command.ExaminationTypeId, ct);
        if (examinationType is null)
            return Result.Failure(Error.NotFound("ExaminationType", command.ExaminationTypeId));

        examinationType.RemoveItem(command.ExaminationTypeItemId);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
