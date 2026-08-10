using RadiologyCenter.Cash.Application.Abstractions;
using RadiologyCenter.Cash.Application.DTOs;

namespace RadiologyCenter.Cash.Application.Queries.Handovers.GetCashHandoverBySession;

public static class GetCashHandoverBySessionQueryHandler
{
    public static async Task<Result<CashHandoverDto?>> HandleAsync(
        GetCashHandoverBySessionQuery query,
        ICashHandoverRepository handoverRepository,
        ICashDirectory directory,
        CancellationToken ct)
    {
        var handover = await handoverRepository.GetBySessionAsync(query.CashSessionId, ct);
        if (handover is null)
            return Result.Success<CashHandoverDto?>(null);

        var userNames = await directory.ResolveUserNamesAsync(new[] { handover.ClosedByUserId }, ct);

        return Result.Success<CashHandoverDto?>(
            handover.ToDto(userNames.GetValueOrDefault(handover.ClosedByUserId) ?? string.Empty));
    }
}