namespace RadiologyCenter.Payroll.Application.Commands.UpdateExaminationFee;

public record UpdateExaminationFeeCommand(
    Guid ExaminationFeeId,
    string Role,
    decimal Amount,
    bool IsPercentage = false) : ICommand;