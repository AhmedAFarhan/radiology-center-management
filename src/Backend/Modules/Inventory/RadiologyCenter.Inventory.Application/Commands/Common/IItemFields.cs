namespace RadiologyCenter.Inventory.Application.Commands.Common;

public interface IItemFields
{
    string Name { get; }
    string Category { get; }
    string Unit { get; }
    string? Brand { get; }
    int ReorderLevel { get; }
    int ReorderQuantity { get; }
    bool LotTracked { get; }
    string? StorageInstructions { get; }
}
