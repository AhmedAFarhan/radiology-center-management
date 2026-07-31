using RadiologyCenter.Inventory.Domain.Entities;

namespace RadiologyCenter.Inventory.Application.Abstractions;

public interface IPurchaseOrderRepository : IBaseRepository<PurchaseOrder, Guid>
{
    Task<PurchaseOrder?> GetWithItemsAsync(Guid id, CancellationToken ct = default);
}
