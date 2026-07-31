namespace RadiologyCenter.Inventory.Application.DTOs;

public record ItemDto(
    Guid Id,
    string Name,
    string? Brand,
    string Category,
    string Unit,
    int ReorderLevel,
    int ReorderQuantity,
    bool LotTracked,
    string? StorageInstructions,
    bool IsActive,
    DateTime CreatedAt);
