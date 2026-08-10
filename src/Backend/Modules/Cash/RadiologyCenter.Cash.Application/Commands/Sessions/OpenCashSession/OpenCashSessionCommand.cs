namespace RadiologyCenter.Cash.Application.Commands.Sessions.OpenCashSession;

public record OpenCashSessionCommand(
    decimal OpeningFloat,
    Guid? WorkShiftId = null,
    string? Notes = null) : ICommand;