namespace RadiologyCenter.Catalog.Application.Commands.ActivateExaminationType;

public record ActivateExaminationTypeCommand(Guid ExaminationTypeId) : ICommand;
