using Mapster;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.DTOs;

namespace RadiologyCenter.Examinations.Application.Commands.AddExaminationItem;

public static class AddExaminationItemCommandHandler
{
    public static async Task<Result<ExaminationItemDto>> HandleAsync(
        AddExaminationItemCommand command,
        IExaminationRepository examinationRepository,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var examination = await examinationRepository.GetWithItemsAsync(command.ExaminationId, ct);
        if (examination is null)
            return Result.Failure<ExaminationItemDto>(Error.NotFound("Examination", command.ExaminationId));

        var item = examination.AddItem(
            command.ItemId,
            command.Quantity,
            command.IsContrast,
            command.IsRequired,
            command.Notes);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(item.Adapt<ExaminationItemDto>());
    }
}
