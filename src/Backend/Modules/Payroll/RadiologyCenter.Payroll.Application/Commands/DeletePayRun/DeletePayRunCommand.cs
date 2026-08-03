namespace RadiologyCenter.Payroll.Application.Commands.DeletePayRun;

public record DeletePayRunCommand(Guid PayRunId) : ICommand;