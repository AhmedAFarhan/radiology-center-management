using Mapster;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.DTOs;

namespace RadiologyCenter.Examinations.Application.Queries.GetExaminationTypes;

public static class GetExaminationTypesQueryHandler
{
    public static async Task<Result<PagedResult<ExaminationTypeDto>>> HandleAsync(
        GetExaminationTypesQuery query,
        IExaminationTypeRepository examinationTypeRepository,
        CancellationToken ct)
    {
        var paged = await examinationTypeRepository.GetPagedAsync(query.Request, ct);
        var dtos = paged.Items.Select(t => t.Adapt<ExaminationTypeDto>()).ToList();

        return Result.Success(new PagedResult<ExaminationTypeDto>(
            dtos,
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize
        ));
    }
}
