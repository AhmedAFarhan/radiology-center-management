namespace RadiologyCenter.Inventory.Application.Commands.IssueStock;

public record IssueStockCommand(
    Guid ItemId,
    int Quantity,
    string? Reference = null,
    string? Notes = null) : ICommand;
