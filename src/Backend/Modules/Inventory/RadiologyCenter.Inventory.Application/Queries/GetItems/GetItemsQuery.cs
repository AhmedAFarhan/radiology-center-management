using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Inventory.Application.Queries.GetItems;

public record GetItemsQuery(QueryRequest Request) : IQuery;
