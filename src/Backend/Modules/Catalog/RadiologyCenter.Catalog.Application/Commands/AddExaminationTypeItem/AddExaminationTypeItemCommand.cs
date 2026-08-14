namespace RadiologyCenter.Catalog.Application.Commands.AddExaminationTypeItem;

public record AddExaminationTypeItemCommand(
    Guid ExaminationTypeId,
    Guid ItemId,
    int Quantity,
    bool IsContrast = false,
    bool IsRequired = false,
    string? Notes = null) : ICommand;
