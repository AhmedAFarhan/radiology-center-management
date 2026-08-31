using Mapster;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.DTOs;
using RadiologyCenter.BuildingBlocks.Application.Localization;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;

namespace RadiologyCenter.Payroll.Application.Queries.GetPayRuns;

public static class GetPayRunsQueryHandler
{
    public static async Task<Result<PagedResult<PayRunListItemDto>>> HandleAsync(
        GetPayRunsQuery query,
        IPayRunRepository repository,
        CancellationToken ct)
    {
        var paged = await repository.GetPagedAsync(query.Request, ct);

        var items = paged.Items.Select(payRun =>
        {
            var payslips = payRun.Payslips;
            return new PayRunListItemDto(
                payRun.Id,
                payRun.RunFrom,
                payRun.RunTo,
                payRun.Status.LocalizedName(),
                null,
                payRun.ProcessedAt,
                payRun.Notes,
                payslips.Count,
                payslips.Sum(p => p.NetSalary),
                payRun.Status.Name);
        }).ToList();

        return Result.Success(new PagedResult<PayRunListItemDto>(
            items,
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize));
    }
}