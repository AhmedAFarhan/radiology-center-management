namespace RadiologyCenter.Examinations.Application.Commands.StartExamination;

public record StartExaminationCommand(
    Guid VisitId,
    Guid ExaminationId) : ICommand;
