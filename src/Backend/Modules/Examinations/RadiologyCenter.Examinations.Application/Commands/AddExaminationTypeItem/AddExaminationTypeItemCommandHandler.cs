using Mapster;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.DTOs;

namespace RadiologyCenter.Examinations.Application.Commands.AddExaminationTypeItem;

public static class AddExaminationTypeItemCommandHandler
{
    public static async Task<Result<ExaminationTypeItemDto>> HandleAsync(
        AddExaminationTypeItemCommand command,
        IExaminationTypeRepository examinationTypeRepository,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var examinationType = await examinationTypeRepository.GetWithItemsAsync(command.ExaminationTypeId, ct);
        if (examinationType is null)
            return Result.Failure<ExaminationTypeItemDto>(Error.NotFound("ExaminationType", command.ExaminationTypeId));

        var item = examinationType.AddItem(
            command.ItemId,
            command.Quantity,
            command.IsContrast,
            command.IsRequired,
            command.Notes);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(item.Adapt<ExaminationTypeItemDto>());
    }
}
