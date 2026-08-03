using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.ResourceManagement.Application.Queries.GetLeaves;

public record GetLeavesQuery(QueryRequest Request) : IQuery;
