namespace RadiologyCenter.Cash.Application.Queries.Sessions.GetCashEntries;

public record GetCashEntriesQuery(Guid CashSessionId) : IQuery;