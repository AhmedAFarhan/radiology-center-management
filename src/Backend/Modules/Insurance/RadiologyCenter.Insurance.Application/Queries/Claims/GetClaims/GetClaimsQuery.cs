using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Insurance.Application.Queries.Claims.GetClaims;

public record GetClaimsQuery(QueryRequest Request) : IQuery;