namespace RadiologyCenter.Inventory.Application.Queries.GetItemById;

public record GetItemByIdQuery(Guid Id) : IQuery, IEntityIdQuery;
