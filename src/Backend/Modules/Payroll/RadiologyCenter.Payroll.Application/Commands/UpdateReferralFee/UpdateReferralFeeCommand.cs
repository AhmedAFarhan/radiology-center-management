namespace RadiologyCenter.Payroll.Application.Commands.UpdateReferralFee;

public record UpdateReferralFeeCommand(
    Guid Id,
    decimal Amount,
    bool IsPercentage = false) : ICommand;