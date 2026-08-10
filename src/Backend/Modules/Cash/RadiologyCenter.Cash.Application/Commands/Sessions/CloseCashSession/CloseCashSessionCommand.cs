namespace RadiologyCenter.Cash.Application.Commands.Sessions.CloseCashSession;

public record CloseCashSessionCommand(
    Guid CashSessionId,
    decimal CountedTotal,
    Guid? ReceivingUserId = null,
    decimal? ReceivingOpeningFloat = null,
    string? Notes = null) : ICommand;