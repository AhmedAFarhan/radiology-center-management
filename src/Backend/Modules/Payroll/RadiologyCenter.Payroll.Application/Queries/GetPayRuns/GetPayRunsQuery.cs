using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Payroll.Application.Queries.GetPayRuns;

public record GetPayRunsQuery(QueryRequest Request) : IQuery;