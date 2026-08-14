namespace RadiologyCenter.Catalog.Application.Commands.DeactivateExaminationType;

public record DeactivateExaminationTypeCommand(Guid ExaminationTypeId) : ICommand;
