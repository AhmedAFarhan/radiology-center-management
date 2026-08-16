using RadiologyCenter.Reports.Application.Abstractions;
using RadiologyCenter.Reports.Application.Localization;
using RadiologyCenter.Reports.Application.DTOs;

namespace RadiologyCenter.Reports.Application.Queries.GetReportById;

public static class GetReportByIdQueryHandler
{
    public static async Task<Result<ReportDto>> HandleAsync(
        GetReportByIdQuery query,
        IReportRepository reportRepository,
        IReportDirectory reportDirectory,
        CancellationToken ct)
    {
        var report = await reportRepository.GetByIdWithVersionsAsync(query.ReportId, ct);
        if (report is null)
            return Result.Failure<ReportDto>(Error.NotFound(ErrorCodes.ReportNotFound, "Report", query.ReportId));

        var dto = report.ToDto();
        dto = await Decorate(dto, reportDirectory, ct);

        return Result.Success(dto);
    }

    public static async Task<ReportDto> Decorate(ReportDto dto, IReportDirectory reportDirectory, CancellationToken ct)
    {
        var patientNames = await reportDirectory.ResolvePatientNamesAsync(new[] { dto.PatientId }, ct);
        var radiologistNames = await reportDirectory.ResolveRadiologistNamesAsync(new[] { dto.RadiologistId }, ct);
        var examinationTypeNames = await reportDirectory.ResolveExaminationTypeNamesAsync(new[] { dto.ExaminationId }, ct);

        return dto with
        {
            PatientName = patientNames.GetValueOrDefault(dto.PatientId),
            RadiologistName = radiologistNames.GetValueOrDefault(dto.RadiologistId),
            ExaminationTypeName = examinationTypeNames.GetValueOrDefault(dto.ExaminationId)
        };
    }
}