using RadiologyCenter.Cash.Application.Abstractions;
using RadiologyCenter.Cash.Domain.Enumerations;
using RadiologyCenter.Examinations.Application.DTOs;
using RadiologyCenter.Examinations.Application.Queries.ExportAnalytics;

namespace RadiologyCenter.Localhost.Extensions;

public sealed class CashFlowDataSource : ICashFlowDataSource
{
    private readonly ICashEntryRepository _entryRepository;
    private readonly ICashSessionRepository _sessionRepository;

    public CashFlowDataSource(ICashEntryRepository entryRepository, ICashSessionRepository sessionRepository)
    {
        _entryRepository = entryRepository;
        _sessionRepository = sessionRepository;
    }

    public async Task<IReadOnlyList<CashFlowPeriodDto>> GetByMonthAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var entries = await _entryRepository.GetByDateRangeAsync(from, to, ct);

        return entries
            .GroupBy(e => $"{e.OccurredAt.Year:0000}-{e.OccurredAt.Month:00}")
            .Select(g => new CashFlowPeriodDto(
                g.Key,
                g.Where(e => e.Direction == CashEntryDirection.In).Sum(e => e.Amount),
                g.Where(e => e.Direction == CashEntryDirection.Out).Sum(e => e.Amount),
                g.Where(e => e.Direction == CashEntryDirection.In).Sum(e => e.Amount) - g.Where(e => e.Direction == CashEntryDirection.Out).Sum(e => e.Amount)))
            .OrderBy(x => x.Month)
            .ToList();
    }

    public async Task<IReadOnlyList<CashFlowEntryTypeDto>> GetByReasonAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var entries = await _entryRepository.GetByDateRangeAsync(from, to, ct);

        return entries
            .GroupBy(e => e.Reason.Name)
            .Select(g => new CashFlowEntryTypeDto(
                g.Key,
                g.Where(e => e.Direction == CashEntryDirection.In).Sum(e => e.Amount),
                g.Where(e => e.Direction == CashEntryDirection.Out).Sum(e => e.Amount),
                g.Count()))
            .OrderByDescending(x => x.EntryCount)
            .ToList();
    }

    public async Task<IReadOnlyList<CashFlowSessionSummaryDto>> GetSessionSummariesAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var sessions = await _sessionRepository.GetByDateRangeAsync(from, to, ct);
        var sessionIds = sessions.Select(s => s.Id).ToList();
        var movements = await _entryRepository.GetSessionMovementsAsync(sessionIds, ct);

        return sessions.Select(s =>
        {
            var m = movements.TryGetValue(s.Id, out var v) ? v : (0m, 0);
            return new CashFlowSessionSummaryDto(
                s.Id,
                "",
                s.OpeningFloat,
                m.Item1,
                m.Item2,
                s.OpenedAt,
                s.ClosedAt,
                s.Status.Name);
        }).ToList();
    }

    public async Task<decimal> GetTotalInflowsAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var entries = await _entryRepository.GetByDateRangeAsync(from, to, ct);
        return entries.Where(e => e.Direction == CashEntryDirection.In).Sum(e => e.Amount);
    }

    public async Task<decimal> GetTotalOutflowsAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var entries = await _entryRepository.GetByDateRangeAsync(from, to, ct);
        return entries.Where(e => e.Direction == CashEntryDirection.Out).Sum(e => e.Amount);
    }

    public async Task<int> GetTotalSessionsAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var sessions = await _sessionRepository.GetByDateRangeAsync(from, to, ct);
        return sessions.Count;
    }

    public async Task<int> GetTotalEntriesAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var entries = await _entryRepository.GetByDateRangeAsync(from, to, ct);
        return entries.Count;
    }
}
