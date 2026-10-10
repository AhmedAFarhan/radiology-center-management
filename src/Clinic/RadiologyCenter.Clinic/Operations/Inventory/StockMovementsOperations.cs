using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Inventory.Application.DTOs;
using RadiologyCenter.Inventory.Application.Queries.GetStockMovements;

using RadiologyCenter.Clinic.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Clinic.Operations.Inventory;

public class StockMovementsOperations : ClinicOperations
{
    private readonly IMessageBus _bus;

    public StockMovementsOperations(IMessageBus bus) => _bus = bus;

    [ClinicPermission(InventoryStockReadCode)]
    [ClinicOperation("POST", "inventory/stock-movements/all")]
    public async Task<ClinicResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<StockMovementDto>>>(new GetStockMovementsQuery(request), ct);
        return result.ToActionResult();
    }
}
