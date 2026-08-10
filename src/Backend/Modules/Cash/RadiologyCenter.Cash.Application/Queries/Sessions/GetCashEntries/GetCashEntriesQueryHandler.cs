using RadiologyCenter.Cash.Application.Abstractions;
using RadiologyCenter.Cash.Application.DTOs;

namespace RadiologyCenter.Cash.Application.Queries.Sessions.GetCashEntries;

public static class GetCashEntriesQueryHandler
{
    public static async Task<Result<IReadOnlyList<CashEntryDto>>> HandleAsync(
        GetCashEntriesQuery query,
        ICashEntryRepository entryRepository,
        CancellationToken ct)
    {
        var entries = await entryRepository.GetBySessionAsync(query.CashSessionId, ct);
        IReadOnlyList<CashEntryDto> dtos = entries.Select(e => e.ToDto()).ToList();
        return Result.Success(dtos);
    }
}