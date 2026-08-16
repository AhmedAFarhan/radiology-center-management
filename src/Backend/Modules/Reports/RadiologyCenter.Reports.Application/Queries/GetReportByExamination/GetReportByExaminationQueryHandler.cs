using RadiologyCenter.Reports.Application.Abstractions;
using RadiologyCenter.Reports.Application.DTOs;
using RadiologyCenter.Reports.Application.Localization;
using RadiologyCenter.Reports.Application.Queries.GetReportById;

namespace RadiologyCenter.Reports.Application.Queries.GetReportByExamination;

public static class GetReportByExaminationQueryHandler
{
    public static async Task<Result<ReportDto>> HandleAsync(
        GetReportByExaminationQuery query,
        IReportRepository reportRepository,
        IReportDirectory reportDirectory,
        CancellationToken ct)
    {
        var report = await reportRepository.GetByExaminationIdAsync(query.ExaminationId, ct);
        if (report is null)
            return Result.Failure<ReportDto>(Error.NotFound(ErrorCodes.ReportNotFound, "Report", query.ExaminationId));

        var dto = report.ToDto();
        dto = await GetReportByIdQueryHandler.Decorate(dto, reportDirectory, ct);

        return Result.Success(dto);
    }
}