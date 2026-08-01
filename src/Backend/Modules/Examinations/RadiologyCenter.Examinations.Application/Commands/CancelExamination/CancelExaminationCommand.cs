namespace RadiologyCenter.Examinations.Application.Commands.CancelExamination;

public record CancelExaminationCommand(
    Guid VisitId,
    Guid ExaminationId,
    string? Reason = null) : ICommand;
