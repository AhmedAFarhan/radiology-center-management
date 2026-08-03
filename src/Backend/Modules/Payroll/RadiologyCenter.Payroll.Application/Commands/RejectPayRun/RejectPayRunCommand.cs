namespace RadiologyCenter.Payroll.Application.Commands.RejectPayRun;

public record RejectPayRunCommand(Guid PayRunId) : ICommand;