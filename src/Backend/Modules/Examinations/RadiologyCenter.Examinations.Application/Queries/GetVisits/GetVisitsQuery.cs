using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Examinations.Application.Queries.GetVisits;

public record GetVisitsQuery(QueryRequest Request) : IQuery;
