namespace RadiologyCenter.Examinations.Application.Commands.StartExamination;

public record StartExaminationCommand(Guid ExaminationId) : ICommand;
