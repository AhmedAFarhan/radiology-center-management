using RadiologyCenter.Inventory.Application.Commands.Common;

namespace RadiologyCenter.Inventory.Application.Commands.CreateItem;

public record CreateItemCommand(
    string Name,
    string Category,
    string Unit,
    string? Brand = null,
    int ReorderLevel = 0,
    int ReorderQuantity = 0,
    bool LotTracked = false,
    string? StorageInstructions = null) : ICommand, IItemFields;
