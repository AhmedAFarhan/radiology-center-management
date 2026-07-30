using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Idnetity.Application.Queries.GetUsers;

public record GetUsersQuery(QueryRequest Request) : IQuery;
