namespace RadiologyCenter.Catalog.Application.Commands.DeleteExaminationType;

public record DeleteExaminationTypeCommand(Guid ExaminationTypeId) : ICommand;
