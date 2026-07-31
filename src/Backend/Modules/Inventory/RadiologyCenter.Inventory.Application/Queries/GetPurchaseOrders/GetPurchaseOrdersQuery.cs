using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Inventory.Application.Queries.GetPurchaseOrders;

public record GetPurchaseOrdersQuery(QueryRequest Request) : IQuery;
