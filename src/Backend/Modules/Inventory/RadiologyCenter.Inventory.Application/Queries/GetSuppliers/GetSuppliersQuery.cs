using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Inventory.Application.Queries.GetSuppliers;

public record GetSuppliersQuery(QueryRequest Request) : IQuery;
