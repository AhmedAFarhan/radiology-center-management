namespace RadiologyCenter.Examinations.Application.Commands.RemoveExaminationItem;

public record RemoveExaminationItemCommand(
    Guid VisitId,
    Guid ExaminationId,
    Guid ExaminationItemId) : ICommand;
