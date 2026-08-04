namespace RadiologyCenter.Payroll.Application.Commands.RestartPayRun;

public record RestartPayRunCommand(Guid PayRunId) : ICommand;