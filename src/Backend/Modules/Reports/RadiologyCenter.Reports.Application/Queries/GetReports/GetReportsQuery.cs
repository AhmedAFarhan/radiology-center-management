using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Reports.Application.Queries.GetReports;

public record GetReportsQuery(QueryRequest Request) : IQuery;