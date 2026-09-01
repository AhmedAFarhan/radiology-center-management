using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.Queries.GetStaffMachineAnalytics;
using RadiologyCenter.Examinations.Application.Reports;

namespace RadiologyCenter.Examinations.Application.Queries.ExportAnalytics;

public static class ExportStaffReportQueryHandler
{
    public static async Task<Result<ReportContentDto>> HandleAsync(
        ExportStaffReportQuery query,
        IExaminationHistoryRepository historyRepository,
        IAncillaryDirectory ancillaryDirectory,
        ITimezoneConverter timezone,
        IAnalyticsReportService reportService,
        IAnalyticsPdfService pdfService,
        CancellationToken ct)
    {
        var staffResult = await GetStaffMachineAnalyticsQueryHandler.HandleAsync(
            new GetStaffMachineAnalyticsQuery(query.From, query.To),
            historyRepository, ancillaryDirectory, ct);
        if (staffResult.IsFailure)
            return Result.Failure<ReportContentDto>(staffResult.Error);

        var today = timezone.GetLocalDate(DateTime.UtcNow);
        var from = query.From ?? today.AddMonths(-1).AddDays(1).ToDateTime(TimeOnly.MinValue);
        var to = query.To ?? today.AddDays(1).ToDateTime(TimeOnly.MinValue);

        var content = query.Format == ReportFormat.Pdf
            ? pdfService.BuildStaffMachinePdf(staffResult.Value, from, to)
            : reportService.ExportStaffMachine(staffResult.Value, from, to);
        return Result.Success(content);
    }
}
