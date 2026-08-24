using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Inventory.Application.Queries.ExportItems;

public record ExportItemsQuery(QueryRequest Request, bool? IsActive = null) : IQuery;
