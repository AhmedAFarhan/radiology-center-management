namespace RadiologyCenter.Examinations.Application.Commands.ActivateExaminationType;

public record ActivateExaminationTypeCommand(Guid ExaminationTypeId) : ICommand;
