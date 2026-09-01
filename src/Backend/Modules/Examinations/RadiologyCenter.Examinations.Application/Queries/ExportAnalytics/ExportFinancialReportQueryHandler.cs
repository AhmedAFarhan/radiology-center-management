using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.Queries.GetFinancialAnalytics;
using RadiologyCenter.Examinations.Application.Reports;

namespace RadiologyCenter.Examinations.Application.Queries.ExportAnalytics;

public static class ExportFinancialReportQueryHandler
{
    public static async Task<Result<ReportContentDto>> HandleAsync(
        ExportFinancialReportQuery query,
        IExaminationRepository examinationRepository,
        IExaminationTypeDirectory examinationTypeDirectory,
        ITimezoneConverter timezone,
        IAnalyticsReportService reportService,
        IAnalyticsPdfService pdfService,
        CancellationToken ct)
    {
        var financialResult = await GetFinancialAnalyticsQueryHandler.HandleAsync(
            new GetFinancialAnalyticsQuery(query.From, query.To),
            examinationRepository, examinationTypeDirectory, timezone, ct);
        if (financialResult.IsFailure)
            return Result.Failure<ReportContentDto>(financialResult.Error);

        var today = timezone.GetLocalDate(DateTime.UtcNow);
        var from = query.From ?? today.AddMonths(-1).AddDays(1).ToDateTime(TimeOnly.MinValue);
        var to = query.To ?? today.AddDays(1).ToDateTime(TimeOnly.MinValue);

        var content = query.Format == ReportFormat.Pdf
            ? pdfService.BuildFinancialPdf(financialResult.Value, from, to)
            : reportService.ExportFinancial(financialResult.Value, from, to);
        return Result.Success(content);
    }
}
