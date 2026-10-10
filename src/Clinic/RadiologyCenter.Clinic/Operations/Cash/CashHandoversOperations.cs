using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Cash.Application.Commands.Sessions.ApproveCashHandover;
using RadiologyCenter.Cash.Application.DTOs;
using RadiologyCenter.Cash.Application.Queries.Handovers.GetCashHandoverBySession;
using RadiologyCenter.Cash.Application.Queries.Handovers.GetCashHandovers;

using RadiologyCenter.Clinic.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Clinic.Operations.Cash;

public class CashHandoversOperations : ClinicOperations
{
    private readonly IMessageBus _bus;

    public CashHandoversOperations(IMessageBus bus) => _bus = bus;

    [ClinicPermission(CashHandoversReadCode)]
    [ClinicOperation("POST", "cash/handovers/all")]
    public async Task<ClinicResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<CashHandoverDto>>>(new GetCashHandoversQuery(request), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(CashHandoversReadCode)]
    [ClinicOperation("GET", "cash/handovers/{sessionId:guid}")]
    public async Task<ClinicResult> GetBySessionAsync(Guid sessionId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<CashHandoverDto?>>(new GetCashHandoverBySessionQuery(sessionId), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(CashHandoversApproveCode)]
    [ClinicOperation("POST", "cash/handovers/{sessionId:guid}/approve")]
    public async Task<ClinicResult> ApproveAsync(Guid sessionId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<CashHandoverDto>>(new ApproveCashHandoverCommand(sessionId), ct);
        return result.ToActionResult();
    }
}