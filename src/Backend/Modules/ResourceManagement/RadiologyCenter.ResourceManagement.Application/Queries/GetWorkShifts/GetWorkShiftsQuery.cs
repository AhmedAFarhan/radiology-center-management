using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.ResourceManagement.Application.Queries.GetWorkShifts;

public record GetWorkShiftsQuery(QueryRequest Request) : IQuery;
