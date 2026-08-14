using RadiologyCenter.Catalog.Application.Commands.Common;

namespace RadiologyCenter.Catalog.Application.Commands.UpdateExaminationType;

public record UpdateExaminationTypeItemRequest(
    Guid ItemId,
    int Quantity,
    bool IsContrast = false,
    bool IsRequired = false,
    string? Notes = null) : IExaminationTypeItemFields;
