namespace RadiologyCenter.Payroll.Application.Commands.PayPayRun;

public record PayPayRunCommand(Guid PayRunId) : ICommand;