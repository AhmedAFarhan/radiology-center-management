using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Inventory.Domain.Events;

public sealed record StockBelowReorderLevelEvent(
    Guid ItemId,
    string ItemName,
    int StockOnHand,
    int ReorderLevel) : DomainEvent;
