using RadiologyCenter.BuildingBlocks.Application.Abstractions;

namespace RadiologyCenter.Reports.Application.Queries.GetReportVersions;

public record GetReportVersionsQuery(Guid ReportId) : IQuery;