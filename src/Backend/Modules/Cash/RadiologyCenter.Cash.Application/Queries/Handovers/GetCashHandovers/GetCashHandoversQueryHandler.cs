using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Cash.Application.Abstractions;
using RadiologyCenter.Cash.Application.DTOs;

namespace RadiologyCenter.Cash.Application.Queries.Handovers.GetCashHandovers;

public static class GetCashHandoversQueryHandler
{
    public static async Task<Result<PagedResult<CashHandoverDto>>> HandleAsync(
        GetCashHandoversQuery query,
        ICashHandoverRepository handoverRepository,
        ICashDirectory directory,
        CancellationToken ct)
    {
        var paged = await handoverRepository.GetPagedAsync(query.Request, ct);

        var userIds = paged.Items.Select(h => h.ClosedByUserId).Distinct().ToList();
        var userNames = await directory.ResolveUserNamesAsync(userIds, ct);

        var items = paged.Items
            .Select(h => h.ToDto(userNames.GetValueOrDefault(h.ClosedByUserId) ?? string.Empty))
            .ToList();

        return Result.Success(new PagedResult<CashHandoverDto>(
            items,
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize));
    }
}