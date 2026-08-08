using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Reports.Application.Abstractions;
using RadiologyCenter.Reports.Application.DTOs;

namespace RadiologyCenter.Reports.Application.Queries.GetReportTemplates;

public static class GetReportTemplatesQueryHandler
{
    public static async Task<Result<PagedResult<ReportTemplateDto>>> HandleAsync(
        GetReportTemplatesQuery query,
        IReportTemplateRepository templateRepository,
        CancellationToken ct)
    {
        var paged = await templateRepository.GetPagedAsync(query.Request, ct);

        var dtos = paged.Items
            .Select(t => t.ToDto())
            .ToList();

        return Result.Success(new PagedResult<ReportTemplateDto>(
            dtos,
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize));
    }
}