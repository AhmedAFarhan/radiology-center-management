using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Cash.Application.Abstractions;
using RadiologyCenter.Cash.Application.DTOs;

namespace RadiologyCenter.Cash.Application.Queries.Sessions.GetMyOpenCashSession;

public static class GetMyOpenCashSessionQueryHandler
{
    public static async Task<Result<CashSessionDto?>> HandleAsync(
        GetMyOpenCashSessionQuery query,
        ICurrentUser currentUser,
        ICashSessionRepository sessionRepository,
        ICashEntryRepository entryRepository,
        CancellationToken ct)
    {
        if (!Guid.TryParse(currentUser.Id, out var userId))
            return Result.Failure<CashSessionDto?>(Error.Unauthorized());

        var session = await sessionRepository.GetOpenSessionByUserAsync(userId, ct);
        if (session is null)
            return Result.Success<CashSessionDto?>(null);

        var movements = await entryRepository.GetSessionMovementsAsync(new[] { session.Id }, ct);
        movements.TryGetValue(session.Id, out var movement);

        return Result.Success<CashSessionDto?>(session.ToDto(
            session.OpeningFloat + movement.Movement,
            currentUser.Name ?? string.Empty,
            movement.Count));
    }
}