using Mapster;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.DTOs;

namespace RadiologyCenter.Payroll.Application.Queries.GetPayRuns;

public static class GetPayRunsQueryHandler
{
    public static async Task<Result<PagedResult<PayRunDto>>> HandleAsync(
        GetPayRunsQuery query,
        IPayRunRepository payRunRepository,
        CancellationToken ct)
    {
        var paged = await payRunRepository.GetPagedAsync(query.Request, ct);
        var dtos = paged.Items.Select(p => p.Adapt<PayRunDto>()).ToList();

        return Result.Success(new PagedResult<PayRunDto>(dtos, paged.TotalCount, paged.PageNumber, paged.PageSize));
    }
}