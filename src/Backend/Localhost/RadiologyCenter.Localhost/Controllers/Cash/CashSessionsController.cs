using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Cash.Application.Commands.Sessions.AddCashEntry;
using RadiologyCenter.Cash.Application.Commands.Sessions.CloseCashSession;
using RadiologyCenter.Cash.Application.Commands.Sessions.OpenCashSession;
using RadiologyCenter.Cash.Application.DTOs;
using RadiologyCenter.Cash.Application.Queries.Sessions.GetCashEntries;
using RadiologyCenter.Cash.Application.Queries.Sessions.GetCashSessionById;
using RadiologyCenter.Cash.Application.Queries.Sessions.GetCashSessions;
using RadiologyCenter.Cash.Application.Queries.Sessions.GetMyOpenCashSession;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Cash;

[ApiController]
[Route("api/cash/sessions")]
public class CashSessionsController : ControllerBase
{
    private readonly IMessageBus _bus;

    public CashSessionsController(IMessageBus bus) => _bus = bus;

    [HasPermission(CashSessionsOpenCode)]
    [HttpPost]
    public async Task<IActionResult> OpenAsync([FromBody] OpenCashSessionCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<CashSessionDto>>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(CashSessionsReadCode)]
    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, [FromQuery] string? status, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<CashSessionDto>>>(new GetCashSessionsQuery(request, status), ct);
        return result.ToActionResult();
    }

    [HasPermission(CashSessionsReadCode)]
    [HttpGet("my-open")]
    public async Task<IActionResult> GetMyOpenAsync(CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<CashSessionDto?>>(new GetMyOpenCashSessionQuery(), ct);
        return result.ToActionResult();
    }

    [HasPermission(CashSessionsReadCode)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<CashSessionDto>>(new GetCashSessionByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(CashEntriesAddCode)]
    [HttpPost("{id:guid}/entries")]
    public async Task<IActionResult> AddEntryAsync(Guid id, [FromBody] AddCashEntryCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<CashEntryDto>>(command with { CashSessionId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(CashSessionsReadCode)]
    [HttpGet("{id:guid}/entries")]
    public async Task<IActionResult> GetEntriesAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<IReadOnlyList<CashEntryDto>>>(new GetCashEntriesQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(CashSessionsCloseCode)]
    [HttpPost("{id:guid}/close")]
    public async Task<IActionResult> CloseAsync(Guid id, [FromBody] CloseCashSessionCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<CashHandoverDto>>(command with { CashSessionId = id }, ct);
        return result.ToActionResult();
    }
}