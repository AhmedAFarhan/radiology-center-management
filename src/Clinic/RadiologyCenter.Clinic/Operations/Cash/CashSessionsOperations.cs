using RadiologyCenter.Clinic.Dispatching;
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

using RadiologyCenter.Clinic.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Clinic.Operations.Cash;

public class CashSessionsOperations : ClinicOperations
{
    private readonly IMessageBus _bus;

    public CashSessionsOperations(IMessageBus bus) => _bus = bus;

    [ClinicPermission(CashSessionsOpenCode)]
    [ClinicOperation("POST", "cash/sessions")]
    public async Task<ClinicResult> OpenAsync([FromBody] OpenCashSessionCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<CashSessionDto>>(command, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(CashSessionsReadCode)]
    [ClinicOperation("POST", "cash/sessions/all")]
    public async Task<ClinicResult> GetAllAsync([FromBody] QueryRequest request, [FromQuery] string? status, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<CashSessionListItemDto>>>(new GetCashSessionsQuery(request, status), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(CashSessionsReadCode)]
    [ClinicOperation("GET", "cash/sessions/my-open")]
    public async Task<ClinicResult> GetMyOpenAsync(CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<CashSessionDto?>>(new GetMyOpenCashSessionQuery(), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(CashSessionsReadCode)]
    [ClinicOperation("GET", "cash/sessions/{id:guid}")]
    public async Task<ClinicResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<CashSessionDto>>(new GetCashSessionByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(CashEntriesAddCode)]
    [ClinicOperation("POST", "cash/sessions/{id:guid}/entries")]
    public async Task<ClinicResult> AddEntryAsync(Guid id, [FromBody] AddCashEntryCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<CashEntryDto>>(command with { CashSessionId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(CashSessionsReadCode)]
    [ClinicOperation("GET", "cash/sessions/{id:guid}/entries")]
    public async Task<ClinicResult> GetEntriesAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<IReadOnlyList<CashEntryDto>>>(new GetCashEntriesQuery(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(CashSessionsCloseCode)]
    [ClinicOperation("POST", "cash/sessions/{id:guid}/close")]
    public async Task<ClinicResult> CloseAsync(Guid id, [FromBody] CloseCashSessionCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<CashHandoverDto>>(command with { CashSessionId = id }, ct);
        return result.ToActionResult();
    }
}