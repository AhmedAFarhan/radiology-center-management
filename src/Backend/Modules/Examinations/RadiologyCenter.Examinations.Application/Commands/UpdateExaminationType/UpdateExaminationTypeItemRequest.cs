namespace RadiologyCenter.Examinations.Application.Commands.UpdateExaminationType;

public record UpdateExaminationTypeItemRequest(
    Guid? ExaminationTypeItemId,
    Guid ItemId,
    int Quantity,
    bool IsContrast = false,
    bool IsRequired = false,
    string? Notes = null);
