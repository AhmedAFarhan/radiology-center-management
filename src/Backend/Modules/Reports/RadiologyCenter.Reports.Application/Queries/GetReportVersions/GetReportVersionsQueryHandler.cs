using Mapster;
using RadiologyCenter.Reports.Application.Abstractions;
using RadiologyCenter.Reports.Application.DTOs;

namespace RadiologyCenter.Reports.Application.Queries.GetReportVersions;

public static class GetReportVersionsQueryHandler
{
    public static async Task<Result<IReadOnlyList<ReportVersionDto>>> HandleAsync(
        GetReportVersionsQuery query,
        IReportRepository reportRepository,
        CancellationToken ct)
    {
        var report = await reportRepository.GetByIdWithVersionsAsync(query.ReportId, ct);
        if (report is null)
            return Result.Failure<IReadOnlyList<ReportVersionDto>>(Error.NotFound("Report", query.ReportId));

        var versions = report.Versions
            .OrderBy(v => v.VersionNumber)
            .Select(v => v.ToDto())
            .ToList();

        return Result.Success<IReadOnlyList<ReportVersionDto>>(versions);
    }
}