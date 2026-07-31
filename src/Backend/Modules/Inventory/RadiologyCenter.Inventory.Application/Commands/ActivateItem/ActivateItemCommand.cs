namespace RadiologyCenter.Inventory.Application.Commands.ActivateItem;

public record ActivateItemCommand(Guid ItemId) : ICommand;
