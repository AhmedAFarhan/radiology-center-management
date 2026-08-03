using RadiologyCenter.Examinations.Application.Commands.Common;

namespace RadiologyCenter.Examinations.Application.Commands.UpdateExaminationType;

public record UpdateExaminationTypeItemRequest(
    Guid ItemId,
    int Quantity,
    bool IsContrast = false,
    bool IsRequired = false,
    string? Notes = null) : IExaminationTypeItemFields;
