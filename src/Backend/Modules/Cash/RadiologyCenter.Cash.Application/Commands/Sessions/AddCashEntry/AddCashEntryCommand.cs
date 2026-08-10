using RadiologyCenter.Cash.Application.Commands.Sessions.Common;

namespace RadiologyCenter.Cash.Application.Commands.Sessions.AddCashEntry;

public record AddCashEntryCommand(
    Guid CashSessionId,
    CashDirectionInput Direction,
    CashReasonInput Reason,
    decimal Amount,
    string? Description = null,
    string? ReferenceId = null) : ICommand;