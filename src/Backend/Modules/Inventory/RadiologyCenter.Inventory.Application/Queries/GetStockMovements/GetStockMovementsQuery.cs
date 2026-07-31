using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Inventory.Application.Queries.GetStockMovements;

public record GetStockMovementsQuery(QueryRequest Request) : IQuery;
