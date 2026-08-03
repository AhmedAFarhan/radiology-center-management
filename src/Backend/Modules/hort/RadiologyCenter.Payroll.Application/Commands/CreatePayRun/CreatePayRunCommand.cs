namespace RadiologyCenter.Payroll.Application.Commands.CreatePayRun;

public record CreatePayRunCommand(
    DateTime RunFrom,
    DateTime RunTo,
    string? Notes = null) : ICommand;