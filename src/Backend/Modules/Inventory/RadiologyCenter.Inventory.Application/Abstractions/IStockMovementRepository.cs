using RadiologyCenter.Inventory.Domain.Entities;

namespace RadiologyCenter.Inventory.Application.Abstractions;

public interface IStockMovementRepository : IBaseRepository<StockMovement, Guid>;
