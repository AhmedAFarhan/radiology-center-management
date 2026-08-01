using Mapster;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.DTOs;

namespace RadiologyCenter.Examinations.Application.Commands.AddExaminationItem;

public static class AddExaminationItemCommandHandler
{
    public static async Task<Result<ExaminationItemDto>> HandleAsync(
        AddExaminationItemCommand command,
        IVisitRepository visitRepository,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var visit = await visitRepository.GetByIdAsync(command.VisitId, ct);
        if (visit is null)
            return Result.Failure<ExaminationItemDto>(Error.NotFound("Visit", command.VisitId));

        var item = visit.AddExaminationItem(
            command.ExaminationId,
            command.ItemId,
            command.Quantity,
            command.IsContrast,
            command.IsRequired,
            command.Notes);

        visitRepository.Update(visit);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(item.Adapt<ExaminationItemDto>());
    }
}
