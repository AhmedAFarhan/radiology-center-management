namespace RadiologyCenter.Examinations.Application.Commands.UpdateExaminationTypeItem;

public record UpdateExaminationTypeItemCommand(
    Guid ExaminationTypeId,
    Guid ExaminationTypeItemId,
    Guid ItemId,
    int Quantity,
    bool IsContrast,
    bool IsRequired,
    string? Notes = null) : ICommand;
