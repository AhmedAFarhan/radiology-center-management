using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Inventory.Application.Commands.ImportItems;

public record ImportItemsCommand(byte[] FileContent) : ICommand;
