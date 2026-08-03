namespace RadiologyCenter.ResourceManagement.Application.DTOs;

public record EquipmentDto(
    Guid Id,
    string Name,
    string? SerialNumber,
    string Modality,
    string Status,
    DateTime? PurchaseDate,
    bool IsActive,
    DateTime CreatedAt);
