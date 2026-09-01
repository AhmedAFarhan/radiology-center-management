using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.Queries.GetOperationalAnalytics;
using RadiologyCenter.Examinations.Application.Reports;

namespace RadiologyCenter.Examinations.Application.Queries.ExportAnalytics;

public static class ExportOperationalReportQueryHandler
{
    public static async Task<Result<ReportContentDto>> HandleAsync(
        ExportOperationalReportQuery query,
        IExaminationRepository examinationRepository,
        IExaminationTypeDirectory examinationTypeDirectory,
        ITimezoneConverter timezone,
        IAnalyticsReportService reportService,
        IAnalyticsPdfService pdfService,
        CancellationToken ct)
    {
        var operationalResult = await GetOperationalAnalyticsQueryHandler.HandleAsync(
            new GetOperationalAnalyticsQuery(query.From, query.To),
            examinationRepository, examinationTypeDirectory, ct);
        if (operationalResult.IsFailure)
            return Result.Failure<ReportContentDto>(operationalResult.Error);

        var today = timezone.GetLocalDate(DateTime.UtcNow);
        var from = query.From ?? today.AddMonths(-1).AddDays(1).ToDateTime(TimeOnly.MinValue);
        var to = query.To ?? today.AddDays(1).ToDateTime(TimeOnly.MinValue);

        var content = query.Format == ReportFormat.Pdf
            ? pdfService.BuildOperationalPdf(operationalResult.Value, from, to)
            : reportService.ExportOperational(operationalResult.Value, from, to);
        return Result.Success(content);
    }
}
