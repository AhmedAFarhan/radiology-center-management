using RadiologyCenter.BuildingBlocks.Application.Abstractions;

namespace RadiologyCenter.Reports.Application.Queries.GetReportTemplateById;

public record GetReportTemplateByIdQuery(Guid TemplateId) : IQuery;