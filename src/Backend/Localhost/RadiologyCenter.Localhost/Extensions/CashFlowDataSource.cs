using Microsoft.EntityFrameworkCore;
using RadiologyCenter.Examinations.Application.DTOs;
using RadiologyCenter.Examinations.Application.Queries.ExportAnalytics;
using RadiologyCenter.Cash.Infrastructure.Persistence;

namespace RadiologyCenter.Localhost.Extensions;

public sealed class CashFlowDataSource : ICashFlowDataSource
{
    private readonly CashDbContext _db;

    public CashFlowDataSource(CashDbContext db) => _db = db;

    public async Task<IReadOnlyList<CashFlowPeriodDto>> GetByMonthAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        return await _db.CashEntries
            .Where(e => e.OccurredAt >= from && e.OccurredAt < to)
            .GroupBy(e => $"{e.OccurredAt.Year:0000}-{e.OccurredAt.Month:00}")
            .Select(g => new CashFlowPeriodDto(
                g.Key,
                g.Where(e => e.Direction.Name == "In").Sum(e => e.Amount),
                g.Where(e => e.Direction.Name == "Out").Sum(e => e.Amount),
                g.Where(e => e.Direction.Name == "In").Sum(e => e.Amount) - g.Where(e => e.Direction.Name == "Out").Sum(e => e.Amount)))
            .OrderBy(x => x.Month)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<CashFlowEntryTypeDto>> GetByReasonAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        return await _db.CashEntries
            .Where(e => e.OccurredAt >= from && e.OccurredAt < to)
            .GroupBy(e => e.Reason.Name)
            .Select(g => new CashFlowEntryTypeDto(
                g.Key,
                g.Where(e => e.Direction.Name == "In").Sum(e => e.Amount),
                g.Where(e => e.Direction.Name == "Out").Sum(e => e.Amount),
                g.Count()))
            .OrderByDescending(x => x.EntryCount)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<CashFlowSessionSummaryDto>> GetSessionSummariesAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        return await _db.CashSessions
            .Where(s => s.OpenedAt >= from && s.OpenedAt < to)
            .Select(s => new CashFlowSessionSummaryDto(
                s.Id,
                "",
                s.OpeningFloat,
                _db.CashEntries.Where(e => e.CashSessionId == s.Id).Sum(e => e.Direction.Name == "In" ? e.Amount : -e.Amount),
                _db.CashEntries.Count(e => e.CashSessionId == s.Id),
                s.OpenedAt,
                s.ClosedAt,
                s.Status.Name))
            .ToListAsync(ct);
    }

    public async Task<decimal> GetTotalInflowsAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => await _db.CashEntries
            .Where(e => e.OccurredAt >= from && e.OccurredAt < to && e.Direction.Name == "In")
            .SumAsync(e => e.Amount, ct);

    public async Task<decimal> GetTotalOutflowsAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => await _db.CashEntries
            .Where(e => e.OccurredAt >= from && e.OccurredAt < to && e.Direction.Name == "Out")
            .SumAsync(e => e.Amount, ct);

    public async Task<int> GetTotalSessionsAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => await _db.CashSessions.CountAsync(s => s.OpenedAt >= from && s.OpenedAt < to, ct);

    public async Task<int> GetTotalEntriesAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => await _db.CashEntries.CountAsync(e => e.OccurredAt >= from && e.OccurredAt < to, ct);
}
