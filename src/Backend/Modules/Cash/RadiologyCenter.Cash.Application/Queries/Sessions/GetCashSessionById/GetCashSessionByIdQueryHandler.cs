using RadiologyCenter.Cash.Application.Localization;
using RadiologyCenter.Cash.Application.Abstractions;
using RadiologyCenter.Cash.Application.DTOs;

namespace RadiologyCenter.Cash.Application.Queries.Sessions.GetCashSessionById;

public static class GetCashSessionByIdQueryHandler
{
    public static async Task<Result<CashSessionDto>> HandleAsync(
        GetCashSessionByIdQuery query,
        ICashSessionRepository sessionRepository,
        ICashEntryRepository entryRepository,
        ICashDirectory directory,
        CancellationToken ct)
    {
        var session = await sessionRepository.GetByIdAsync(query.Id, ct);
        if (session is null)
            return Result.Failure<CashSessionDto>(Error.NotFound(ErrorCodes.SessionNotFound, "CashSession", query.Id));

        var movements = await entryRepository.GetSessionMovementsAsync(new[] { session.Id }, ct);
        movements.TryGetValue(session.Id, out var movement);

        var userNames = await directory.ResolveUserNamesAsync(new[] { session.UserId }, ct);

        return Result.Success(session.ToDto(
            session.OpeningFloat + movement.Movement,
            userNames.GetValueOrDefault(session.UserId) ?? string.Empty,
            movement.Count));
    }
}