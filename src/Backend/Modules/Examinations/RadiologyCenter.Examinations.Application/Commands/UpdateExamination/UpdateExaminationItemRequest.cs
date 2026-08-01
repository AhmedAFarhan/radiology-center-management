namespace RadiologyCenter.Examinations.Application.Commands.UpdateExamination;

public record UpdateExaminationItemRequest(
    Guid? ExaminationItemId,
    Guid ItemId,
    int Quantity,
    bool IsContrast = false,
    bool IsRequired = false,
    string? Notes = null);
