namespace RadiologyCenter.Examinations.Application.Commands.CancelExamination;

public record CancelExaminationCommand(
    Guid ExaminationId,
    string? Reason = null) : ICommand;
