using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Cash.Application.Queries.Sessions.GetCashSessions;

public record GetCashSessionsQuery(QueryRequest Request, string? Status = null) : IQuery;