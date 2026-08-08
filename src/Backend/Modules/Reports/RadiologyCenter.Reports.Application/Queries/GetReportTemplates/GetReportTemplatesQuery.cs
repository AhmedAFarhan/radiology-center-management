using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Reports.Application.Queries.GetReportTemplates;

public record GetReportTemplatesQuery(QueryRequest Request) : IQuery;