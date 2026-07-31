namespace RadiologyCenter.Inventory.Application.Commands.UpdateItem;

public record UpdateItemCommand(
    Guid ItemId,
    string Name,
    string Category,
    string Unit,
    string? Brand = null,
    int ReorderLevel = 0,
    int ReorderQuantity = 0,
    bool LotTracked = false,
    string? StorageInstructions = null) : ICommand;
