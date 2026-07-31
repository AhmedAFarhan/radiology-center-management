using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Identity.Application.Queries.GetUsers;

public record GetUsersQuery(QueryRequest Request) : IQuery;
