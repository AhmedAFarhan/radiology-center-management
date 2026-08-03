namespace RadiologyCenter.Payroll.Application.Commands.ComputePayRun;

public record ComputePayRunCommand(Guid PayRunId) : ICommand;