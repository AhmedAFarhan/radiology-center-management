namespace RadiologyCenter.Inventory.Application.Queries.GetItemStock;

public record GetItemStockQuery(Guid ItemId) : IQuery;
