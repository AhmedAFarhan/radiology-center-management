using RadiologyCenter.Examinations.Application.Commands.Common;

namespace RadiologyCenter.Examinations.Application.Commands.CreateExaminationType;

public record CreateExaminationTypeItem(
    Guid ItemId,
    int Quantity,
    bool IsContrast = false,
    bool IsRequired = false,
    string? Notes = null) : IExaminationTypeItemFields;
