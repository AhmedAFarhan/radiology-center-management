using Mapster;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.DTOs;

namespace RadiologyCenter.Examinations.Application.Queries.GetExaminations;

public static class GetExaminationsQueryHandler
{
    public static async Task<Result<PagedResult<ExaminationListItemDto>>> HandleAsync(
        GetExaminationsQuery query,
        IExaminationRepository examinationRepository,
        IExaminationTypeDirectory examinationTypeDirectory,
        CancellationToken ct)
    {
        var paged = await examinationRepository.GetPagedWithItemsAsync(query.Request, ct);

        var typeIds = paged.Items.Select(e => e.ExaminationTypeId).Distinct().ToList();
        var types = await examinationTypeDirectory.GetWithItemsByIdsAsync(typeIds, ct);
        var typeLookup = types.ToDictionary(t => t.Id);

        var dtos = paged.Items
            .Select(e => e.ToListItemDto(typeLookup.TryGetValue(e.ExaminationTypeId, out var t) ? t.Name : string.Empty))
            .ToList();

        return Result.Success(new PagedResult<ExaminationListItemDto>(
            dtos,
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize
        ));
    }
}
