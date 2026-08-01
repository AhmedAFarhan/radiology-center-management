namespace RadiologyCenter.Examinations.Application.Commands.DeleteExaminationType;

public record DeleteExaminationTypeCommand(Guid ExaminationTypeId) : ICommand;
