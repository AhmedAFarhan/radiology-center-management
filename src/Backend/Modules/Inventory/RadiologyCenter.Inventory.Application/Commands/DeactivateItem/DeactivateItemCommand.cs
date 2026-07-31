namespace RadiologyCenter.Inventory.Application.Commands.DeactivateItem;

public record DeactivateItemCommand(Guid ItemId) : ICommand;
