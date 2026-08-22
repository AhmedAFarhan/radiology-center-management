using Mapster;
using RadiologyCenter.BuildingBlocks.Application.Localization;
using RadiologyCenter.Inventory.Application.DTOs;
using RadiologyCenter.Inventory.Domain.Entities;

namespace RadiologyCenter.Inventory.Application;

public static class InventoryMappingConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<Item, ItemDto>.NewConfig()
            .Map(d => d.Category, s => s.Category.LocalizedName())
            .Map(d => d.CategoryKey, s => s.Category.Name)
            .Map(d => d.Unit, s => s.Unit.LocalizedName())
            .Map(d => d.UnitKey, s => s.Unit.Name);

        TypeAdapterConfig<StockBatch, StockBatchDto>.NewConfig()
            .Map(d => d.IsExpired, s => s.IsExpired(DateTime.UtcNow));

        TypeAdapterConfig<StockMovement, StockMovementDto>.NewConfig()
            .Map(d => d.MovementType, s => s.MovementType.LocalizedName())
            .Map(d => d.MovementTypeKey, s => s.MovementType.Name);

        TypeAdapterConfig<PurchaseOrder, PurchaseOrderDto>.NewConfig()
            .Map(d => d.Status, s => s.Status.LocalizedName());

        TypeAdapterConfig<PurchaseOrderItem, PurchaseOrderItemDto>.NewConfig();
    }
}
