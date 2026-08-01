namespace RadiologyCenter.Examinations.Application.Commands.RemoveExaminationItem;

public record RemoveExaminationItemCommand(
    Guid ExaminationId,
    Guid ExaminationItemId) : ICommand;
