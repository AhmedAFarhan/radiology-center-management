using RadiologyCenter.Reports.Application.Abstractions;
using RadiologyCenter.Reports.Application.DTOs;

namespace RadiologyCenter.Reports.Application.Queries.GetReportById;

public static class GetReportByIdQueryHandler
{
    public static async Task<Result<ReportDto>> HandleAsync(
        GetReportByIdQuery query,
        IReportRepository reportRepository,
        CancellationToken ct)
    {
        var report = await reportRepository.GetByIdWithVersionsAsync(query.ReportId, ct);
        if (report is null)
            return Result.Failure<ReportDto>(Error.NotFound("Report", query.ReportId));

        return Result.Success(report.ToDto());
    }
}