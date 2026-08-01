namespace RadiologyCenter.Examinations.Application.Commands.CheckInExamination;

public record CheckInExaminationCommand(Guid ExaminationId) : ICommand;
