namespace RadiologyCenter.Cash.Application.Commands.Sessions.ApproveCashHandover;

public record ApproveCashHandoverCommand(Guid CashSessionId) : ICommand;