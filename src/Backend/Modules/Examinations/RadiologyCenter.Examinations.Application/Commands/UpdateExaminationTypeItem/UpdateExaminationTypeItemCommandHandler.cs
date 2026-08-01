using RadiologyCenter.Examinations.Application.Abstractions;

namespace RadiologyCenter.Examinations.Application.Commands.UpdateExaminationTypeItem;

public static class UpdateExaminationTypeItemCommandHandler
{
    public static async Task<Result> HandleAsync(
        UpdateExaminationTypeItemCommand command,
        IExaminationTypeRepository examinationTypeRepository,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var examinationType = await examinationTypeRepository.GetWithItemsAsync(command.ExaminationTypeId, ct);
        if (examinationType is null)
            return Result.Failure(Error.NotFound("ExaminationType", command.ExaminationTypeId));

        examinationType.UpdateItem(
            command.ExaminationTypeItemId,
            command.ItemId,
            command.Quantity,
            command.IsContrast,
            command.IsRequired,
            command.Notes);

        examinationTypeRepository.Update(examinationType);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
