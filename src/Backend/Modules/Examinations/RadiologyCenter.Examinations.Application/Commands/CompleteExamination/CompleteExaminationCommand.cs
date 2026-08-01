namespace RadiologyCenter.Examinations.Application.Commands.CompleteExamination;

public record CompleteExaminationCommand(Guid ExaminationId) : ICommand;
