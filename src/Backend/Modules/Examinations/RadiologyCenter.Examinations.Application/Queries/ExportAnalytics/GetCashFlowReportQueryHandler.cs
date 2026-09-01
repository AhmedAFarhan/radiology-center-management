using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Examinations.Application.DTOs;

namespace RadiologyCenter.Examinations.Application.Queries.ExportAnalytics;

public static class GetCashFlowReportQueryHandler
{
    public static async Task<Result<CashFlowReportDto>> HandleAsync(
        GetCashFlowReportQuery query,
        ICashFlowDataSource dataSource,
        ITimezoneConverter timezone,
        CancellationToken ct)
    {
        var today = timezone.GetLocalDate(DateTime.UtcNow);
        var from = query.From?.Date ?? today.AddMonths(-1).AddDays(1).ToDateTime(TimeOnly.MinValue);
        var to = query.To?.Date.AddDays(1) ?? today.AddDays(1).ToDateTime(TimeOnly.MinValue);

        var fromUtc = timezone.ToUtc(from);
        var toUtc = timezone.ToUtc(to);

        var totalInflows = await dataSource.GetTotalInflowsAsync(fromUtc, toUtc, ct);
        var totalOutflows = await dataSource.GetTotalOutflowsAsync(fromUtc, toUtc, ct);
        var totalSessions = await dataSource.GetTotalSessionsAsync(fromUtc, toUtc, ct);
        var totalEntries = await dataSource.GetTotalEntriesAsync(fromUtc, toUtc, ct);
        var byMonth = await dataSource.GetByMonthAsync(fromUtc, toUtc, ct);
        var byReason = await dataSource.GetByReasonAsync(fromUtc, toUtc, ct);
        var sessionSummaries = await dataSource.GetSessionSummariesAsync(fromUtc, toUtc, ct);

        var avgBalance = totalSessions == 0 ? 0m : totalInflows / totalSessions;

        return Result.Success(new CashFlowReportDto(
            totalInflows, totalOutflows, totalInflows - totalOutflows,
            totalSessions, totalEntries, avgBalance,
            byMonth, byReason, sessionSummaries));
    }
}
