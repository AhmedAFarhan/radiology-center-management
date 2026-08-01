namespace RadiologyCenter.Examinations.Application.Commands.AddExaminationItem;

public record AddExaminationItemCommand(
    Guid VisitId,
    Guid ExaminationId,
    Guid ItemId,
    int Quantity,
    bool IsContrast = false,
    bool IsRequired = false,
    string? Notes = null) : ICommand;
