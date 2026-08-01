namespace RadiologyCenter.Examinations.Application.Commands.CheckInExamination;

public record CheckInExaminationCommand(
    Guid VisitId,
    Guid ExaminationId) : ICommand;
