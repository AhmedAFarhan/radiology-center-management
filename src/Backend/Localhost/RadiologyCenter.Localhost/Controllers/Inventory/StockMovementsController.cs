using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Inventory.Application.DTOs;
using RadiologyCenter.Inventory.Application.Queries.GetStockMovements;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Inventory;

[ApiController]
[Route("api/inventory/stock-movements")]
public class StockMovementsController : ControllerBase
{
    private readonly IMessageBus _bus;

    public StockMovementsController(IMessageBus bus) => _bus = bus;

    [HasPermission(InventoryStockReadCode)]
    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<StockMovementDto>>>(new GetStockMovementsQuery(request), ct);
        return result.ToActionResult();
    }
}
