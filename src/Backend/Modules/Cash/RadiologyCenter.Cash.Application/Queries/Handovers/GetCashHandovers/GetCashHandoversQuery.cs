using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Cash.Application.Queries.Handovers.GetCashHandovers;

public record GetCashHandoversQuery(QueryRequest Request) : IQuery;