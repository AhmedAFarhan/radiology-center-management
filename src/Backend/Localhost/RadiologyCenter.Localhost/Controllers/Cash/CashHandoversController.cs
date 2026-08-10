using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Cash.Application.Commands.Sessions.ApproveCashHandover;
using RadiologyCenter.Cash.Application.DTOs;
using RadiologyCenter.Cash.Application.Queries.Handovers.GetCashHandoverBySession;
using RadiologyCenter.Cash.Application.Queries.Handovers.GetCashHandovers;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Cash;

[ApiController]
[Route("api/cash/handovers")]
public class CashHandoversController : ControllerBase
{
    private readonly IMessageBus _bus;

    public CashHandoversController(IMessageBus bus) => _bus = bus;

    [HasPermission(CashHandoversReadCode)]
    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<CashHandoverDto>>>(new GetCashHandoversQuery(request), ct);
        return result.ToActionResult();
    }

    [HasPermission(CashHandoversReadCode)]
    [HttpGet("{sessionId:guid}")]
    public async Task<IActionResult> GetBySessionAsync(Guid sessionId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<CashHandoverDto?>>(new GetCashHandoverBySessionQuery(sessionId), ct);
        return result.ToActionResult();
    }

    [HasPermission(CashHandoversApproveCode)]
    [HttpPost("{sessionId:guid}/approve")]
    public async Task<IActionResult> ApproveAsync(Guid sessionId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<CashHandoverDto>>(new ApproveCashHandoverCommand(sessionId), ct);
        return result.ToActionResult();
    }
}