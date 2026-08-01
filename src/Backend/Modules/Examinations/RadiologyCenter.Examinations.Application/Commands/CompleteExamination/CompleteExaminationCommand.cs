namespace RadiologyCenter.Examinations.Application.Commands.CompleteExamination;

public record CompleteExaminationCommand(
    Guid VisitId,
    Guid ExaminationId) : ICommand;
