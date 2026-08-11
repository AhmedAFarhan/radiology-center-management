namespace RadiologyCenter.Examinations.Application.Commands.RecordExaminationPayment;

public record RecordExaminationPaymentCommand(
    Guid ExaminationId,
    decimal Amount,
    string? Description = null) : ICommand;