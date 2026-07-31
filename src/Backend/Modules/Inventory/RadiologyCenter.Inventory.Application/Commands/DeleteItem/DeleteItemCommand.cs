namespace RadiologyCenter.Inventory.Application.Commands.DeleteItem;

public record DeleteItemCommand(Guid ItemId) : ICommand;
