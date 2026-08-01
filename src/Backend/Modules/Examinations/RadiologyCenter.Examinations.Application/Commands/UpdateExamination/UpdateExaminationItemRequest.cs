namespace RadiologyCenter.Examinations.Application.Commands.UpdateExamination;

public record UpdateExaminationItemRequest(
    Guid ItemId,
    int Quantity,
    bool IsContrast = false,
    bool IsRequired = false,
    string? Notes = null);
