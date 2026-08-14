namespace RadiologyCenter.Catalog.Application.DTOs;

public record ExaminationTypeItemDto(
    Guid Id,
    Guid ItemId,
    int Quantity,
    bool IsContrast,
    bool IsRequired,
    string? Notes);
