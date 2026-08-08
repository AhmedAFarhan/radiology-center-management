using RadiologyCenter.Reports.Application.Abstractions;
using RadiologyCenter.Reports.Application.DTOs;

namespace RadiologyCenter.Reports.Application.Queries.GetReportTemplateById;

public static class GetReportTemplateByIdQueryHandler
{
    public static async Task<Result<ReportTemplateDto>> HandleAsync(
        GetReportTemplateByIdQuery query,
        IReportTemplateRepository templateRepository,
        CancellationToken ct)
    {
        var template = await templateRepository.GetByIdWithSectionsAsync(query.TemplateId, ct);
        if (template is null)
            return Result.Failure<ReportTemplateDto>(Error.NotFound("ReportTemplate", query.TemplateId));

        return Result.Success(template.ToDto());
    }
}