using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Identity.Application.Queries.GetRoles;

public record GetRolesQuery(QueryRequest Request) : IQuery;
