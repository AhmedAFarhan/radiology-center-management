using RadiologyCenter.BuildingBlocks.Application.Abstractions;

namespace RadiologyCenter.Reports.Application.Queries.GetReportById;

public record GetReportByIdQuery(Guid ReportId) : IQuery;