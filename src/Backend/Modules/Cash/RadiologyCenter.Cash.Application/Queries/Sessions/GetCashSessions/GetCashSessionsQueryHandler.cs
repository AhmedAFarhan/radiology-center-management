using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Cash.Application.Abstractions;
using RadiologyCenter.Cash.Application.DTOs;
using RadiologyCenter.Cash.Domain.Enumerations;

namespace RadiologyCenter.Cash.Application.Queries.Sessions.GetCashSessions;

public static class GetCashSessionsQueryHandler
{
    public static async Task<Result<PagedResult<CashSessionDto>>> HandleAsync(
        GetCashSessionsQuery query,
        ICashSessionRepository sessionRepository,
        ICashEntryRepository entryRepository,
        ICashDirectory directory,
        CancellationToken ct)
    {
        var status = string.IsNullOrWhiteSpace(query.Status)
            ? null
            : CashSessionStatus.GetAll<CashSessionStatus>().FirstOrDefault(s => s.Name == query.Status);

        var paged = await sessionRepository.GetPagedWithStatusAsync(query.Request, status, ct);

        var userIds = paged.Items.Select(s => s.UserId).Distinct().ToList();
        var userNames = await directory.ResolveUserNamesAsync(userIds, ct);

        var sessionIds = paged.Items.Select(s => s.Id).ToList();
        var movements = await entryRepository.GetSessionMovementsAsync(sessionIds, ct);

        var items = paged.Items.Select(s =>
        {
            movements.TryGetValue(s.Id, out var movement);
            return s.ToDto(
                s.OpeningFloat + movement.Movement,
                userNames.GetValueOrDefault(s.UserId) ?? string.Empty,
                movement.Count);
        }).ToList();

        return Result.Success(new PagedResult<CashSessionDto>(
            items,
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize));
    }
}