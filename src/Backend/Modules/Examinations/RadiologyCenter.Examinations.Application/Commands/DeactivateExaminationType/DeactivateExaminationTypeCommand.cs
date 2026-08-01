namespace RadiologyCenter.Examinations.Application.Commands.DeactivateExaminationType;

public record DeactivateExaminationTypeCommand(Guid ExaminationTypeId) : ICommand;
