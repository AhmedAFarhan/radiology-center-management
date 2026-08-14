using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.DTOs;

namespace RadiologyCenter.Examinations.Application.Queries.GetExaminationTypeItems;

public static class GetExaminationTypeItemsQueryHandler
{
    public static async Task<Result<IReadOnlyList<ExaminationTypeItemDto>>> HandleAsync(
        GetExaminationTypeItemsQuery query,
        IExaminationTypeItemRepository itemRepository,
        CancellationToken ct)
    {
        var items = await itemRepository.GetByTypeIdAsync(query.ExaminationTypeId, ct);
        var dtos = items
            .Select(i => new ExaminationTypeItemDto(i.Id, i.ItemId, i.Quantity, i.IsContrast, i.IsRequired, i.Notes))
            .ToList();
        return Result.Success<IReadOnlyList<ExaminationTypeItemDto>>(dtos);
    }
}

public record GetExaminationTypeItemsQuery(Guid ExaminationTypeId);