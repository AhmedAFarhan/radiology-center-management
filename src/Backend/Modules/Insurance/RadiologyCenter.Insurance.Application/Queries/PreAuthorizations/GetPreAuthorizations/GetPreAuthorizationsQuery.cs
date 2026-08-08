using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Insurance.Application.Queries.PreAuthorizations.GetPreAuthorizations;

public record GetPreAuthorizationsQuery(QueryRequest Request) : IQuery;