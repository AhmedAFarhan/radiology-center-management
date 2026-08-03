namespace RadiologyCenter.Payroll.Application.Commands.CreateExaminationFee;

public record CreateExaminationFeeCommand(
    Guid ExaminationTypeId,
    string Role,
    decimal Amount,
    bool IsPercentage = false) : ICommand;