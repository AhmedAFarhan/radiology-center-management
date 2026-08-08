using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Reports.Application.Abstractions;
using RadiologyCenter.Reports.Application.DTOs;

namespace RadiologyCenter.Reports.Application.Queries.GetReports;

public static class GetReportsQueryHandler
{
    public static async Task<Result<PagedResult<ReportListItemDto>>> HandleAsync(
        GetReportsQuery query,
        IReportRepository reportRepository,
        IReportDirectory reportDirectory,
        CancellationToken ct)
    {
        var paged = await reportRepository.GetPagedAsync(query.Request, ct);

        var dtos = paged.Items
            .Select(r => r.ToListItemDto())
            .ToList();

        if (dtos.Count > 0)
            dtos = await DecorateWithNamesAsync(dtos, reportDirectory, ct);

        return Result.Success(new PagedResult<ReportListItemDto>(
            dtos,
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize));
    }

    private static async Task<List<ReportListItemDto>> DecorateWithNamesAsync(
        List<ReportListItemDto> dtos,
        IReportDirectory reportDirectory,
        CancellationToken ct)
    {
        var patientIds = dtos.Select(d => d.PatientId).Distinct().ToList();
        var radiologistIds = dtos.Select(d => d.RadiologistId).Distinct().ToList();
        var examinationIds = dtos.Select(d => d.ExaminationId).Distinct().ToList();

        var patientNames = await reportDirectory.ResolvePatientNamesAsync(patientIds, ct);
        var radiologistNames = await reportDirectory.ResolveRadiologistNamesAsync(radiologistIds, ct);
        var examinationTypeNames = await reportDirectory.ResolveExaminationTypeNamesAsync(examinationIds, ct);

        return dtos
            .Select(d => d with
            {
                PatientName = patientNames.GetValueOrDefault(d.PatientId),
                RadiologistName = radiologistNames.GetValueOrDefault(d.RadiologistId),
                ExaminationTypeName = examinationTypeNames.GetValueOrDefault(d.ExaminationId)
            })
            .ToList();
    }
}