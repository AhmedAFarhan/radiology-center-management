using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.DTOs;

namespace RadiologyCenter.Examinations.Application.Queries.GetVisits;

public static class GetVisitsQueryHandler
{
    public static async Task<Result<PagedResult<VisitDto>>> HandleAsync(
        GetVisitsQuery query,
        IVisitRepository visitRepository,
        IExaminationTypeRepository examinationTypeRepository,
        CancellationToken ct)
    {
        var paged = await visitRepository.GetPagedWithExaminationsAsync(query.Request, ct);

        var examinationTypeIds = paged.Items
            .SelectMany(v => v.Examinations)
            .Select(e => e.ExaminationTypeId)
            .ToList();
        var typeNames = await VisitMapper.LoadExaminationTypeNamesAsync(examinationTypeIds, examinationTypeRepository, ct);

        var dtos = paged.Items.Select(v => VisitMapper.Map(v, typeNames)).ToList();

        return Result.Success(new PagedResult<VisitDto>(
            dtos,
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize
        ));
    }
}
