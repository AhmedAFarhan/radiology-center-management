using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Reports.Application.Abstractions;
using RadiologyCenter.Reports.Application.DTOs;

namespace RadiologyCenter.Reports.Application.Queries.GetReports;

public static class GetReportsQueryHandler
{
    public static async Task<Result<PagedResult<ReportListItemDto>>> HandleAsync(
        GetReportsQuery query,
        IReportRepository reportRepository,
        CancellationToken ct)
    {
        var paged = await reportRepository.GetPagedAsync(query.Request, ct);

        var dtos = paged.Items
            .Select(r => r.ToListItemDto())
            .ToList();

        return Result.Success(new PagedResult<ReportListItemDto>(
            dtos,
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize));
    }
}