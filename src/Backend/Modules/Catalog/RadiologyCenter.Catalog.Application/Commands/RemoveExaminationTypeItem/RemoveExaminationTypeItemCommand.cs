namespace RadiologyCenter.Catalog.Application.Commands.RemoveExaminationTypeItem;

public record RemoveExaminationTypeItemCommand(
    Guid ExaminationTypeId,
    Guid ExaminationTypeItemId) : ICommand;
