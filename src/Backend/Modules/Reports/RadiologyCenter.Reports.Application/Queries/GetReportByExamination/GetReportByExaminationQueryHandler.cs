using RadiologyCenter.Reports.Application.Abstractions;
using RadiologyCenter.Reports.Application.DTOs;

namespace RadiologyCenter.Reports.Application.Queries.GetReportByExamination;

public static class GetReportByExaminationQueryHandler
{
    public static async Task<Result<ReportDto>> HandleAsync(
        GetReportByExaminationQuery query,
        IReportRepository reportRepository,
        CancellationToken ct)
    {
        var report = await reportRepository.GetByExaminationIdAsync(query.ExaminationId, ct);
        if (report is null)
            return Result.Failure<ReportDto>(Error.NotFound("Report", query.ExaminationId));

        return Result.Success(report.ToDto());
    }
}