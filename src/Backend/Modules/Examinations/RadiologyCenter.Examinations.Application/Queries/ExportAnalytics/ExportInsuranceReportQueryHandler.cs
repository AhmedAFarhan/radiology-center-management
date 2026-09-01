using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Examinations.Application.Queries.ExportAnalytics;
using RadiologyCenter.Examinations.Application.Reports;

namespace RadiologyCenter.Examinations.Application.Queries.ExportAnalytics;

public static class ExportInsuranceReportQueryHandler
{
    public static async Task<Result<ReportContentDto>> HandleAsync(
        ExportInsuranceReportQuery query,
        IInsuranceAnalyticsDataSource dataSource,
        ITimezoneConverter timezone,
        IAnalyticsReportService reportService,
        CancellationToken ct)
    {
        var insuranceResult = await GetInsuranceAnalyticsQueryHandler.HandleAsync(
            new GetInsuranceAnalyticsQuery(query.From, query.To),
            dataSource, timezone, ct);
        if (insuranceResult.IsFailure)
            return Result.Failure<ReportContentDto>(insuranceResult.Error);

        var today = timezone.GetLocalDate(DateTime.UtcNow);
        var from = query.From ?? today.AddMonths(-1).AddDays(1).ToDateTime(TimeOnly.MinValue);
        var to = query.To ?? today.AddDays(1).ToDateTime(TimeOnly.MinValue);

        var content = reportService.ExportInsurance(insuranceResult.Value, from, to);
        return Result.Success(content);
    }
}
