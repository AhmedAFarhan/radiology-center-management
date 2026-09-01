using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Examinations.Application.Reports;

namespace RadiologyCenter.Examinations.Application.Queries.ExportAnalytics;

public static class ExportCashFlowReportQueryHandler
{
    public static async Task<Result<ReportContentDto>> HandleAsync(
        ExportCashFlowReportQuery query,
        ICashFlowDataSource dataSource,
        ITimezoneConverter timezone,
        IAnalyticsReportService reportService,
        CancellationToken ct)
    {
        var cashFlowResult = await GetCashFlowReportQueryHandler.HandleAsync(
            new GetCashFlowReportQuery(query.From, query.To),
            dataSource, timezone, ct);
        if (cashFlowResult.IsFailure)
            return Result.Failure<ReportContentDto>(cashFlowResult.Error);

        var today = timezone.GetLocalDate(DateTime.UtcNow);
        var from = query.From ?? today.AddMonths(-1).AddDays(1).ToDateTime(TimeOnly.MinValue);
        var to = query.To ?? today.AddDays(1).ToDateTime(TimeOnly.MinValue);

        var content = reportService.ExportCashFlow(cashFlowResult.Value, from, to);
        return Result.Success(content);
    }
}
