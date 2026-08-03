namespace RadiologyCenter.Payroll.Application.Commands.CreateReferralFee;

public record CreateReferralFeeCommand(
    Guid ReferralDoctorId,
    Guid ExaminationTypeId,
    decimal Amount,
    bool IsPercentage = false) : ICommand;