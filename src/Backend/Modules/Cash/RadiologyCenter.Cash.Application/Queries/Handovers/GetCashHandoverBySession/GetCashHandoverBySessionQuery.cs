namespace RadiologyCenter.Cash.Application.Queries.Handovers.GetCashHandoverBySession;

public record GetCashHandoverBySessionQuery(Guid CashSessionId) : IQuery;