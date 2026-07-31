using RadiologyCenter.Inventory.Domain.Entities;

namespace RadiologyCenter.Inventory.Application.Abstractions;

public interface IItemRepository : IBaseRepository<Item, Guid>;
