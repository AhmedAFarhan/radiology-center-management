using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.Queries.GetMonthlyProfit;
using RadiologyCenter.Examinations.Application.Reports;

namespace RadiologyCenter.Examinations.Application.Queries.ExportAnalytics;

public static class ExportProfitReportQueryHandler
{
    public static async Task<Result<ReportContentDto>> HandleAsync(
        ExportProfitReportQuery query,
        IExaminationHistoryRepository historyRepository,
        IProfitSourceResolver profitSourceResolver,
        ITimezoneConverter timezone,
        IAnalyticsReportService reportService,
        CancellationToken ct)
    {
        var profitResult = await GetMonthlyProfitQueryHandler.HandleAsync(
            new GetMonthlyProfitQuery(query.From, query.To),
            historyRepository, profitSourceResolver, timezone, ct);
        if (profitResult.IsFailure)
            return Result.Failure<ReportContentDto>(profitResult.Error);

        var today = timezone.GetLocalDate(DateTime.UtcNow);
        var from = query.From ?? today.AddMonths(-1).AddDays(1).ToDateTime(TimeOnly.MinValue);
        var to = query.To ?? today.AddDays(1).ToDateTime(TimeOnly.MinValue);

        var content = reportService.ExportProfit(profitResult.Value, from, to);
        return Result.Success(content);
    }
}
