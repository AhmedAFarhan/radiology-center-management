namespace RadiologyCenter.Payroll.Application.Commands.ApprovePayRun;

public record ApprovePayRunCommand(Guid PayRunId) : ICommand;