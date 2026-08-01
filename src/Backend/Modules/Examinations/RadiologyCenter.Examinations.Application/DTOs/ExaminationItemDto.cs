namespace RadiologyCenter.Examinations.Application.DTOs;

public record ExaminationItemDto(
    Guid Id,
    Guid ItemId,
    int Quantity,
    bool IsContrast,
    bool IsRequired,
    string? Notes);
