namespace RadiologyCenter.Examinations.Application.Commands.UpdateExaminationItem;

public record UpdateExaminationItemCommand(
    Guid VisitId,
    Guid ExaminationId,
    Guid ExaminationItemId,
    int Quantity,
    bool IsContrast,
    bool IsRequired,
    string? Notes = null) : ICommand;
