using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Insurance.Application.Commands.Claims.AdjudicateClaim;
using RadiologyCenter.Insurance.Application.Commands.Claims.CreateClaim;
using RadiologyCenter.Insurance.Application.Commands.Claims.RecordClaimSettlement;
using RadiologyCenter.Insurance.Application.Commands.Claims.ResubmitClaim;
using RadiologyCenter.Insurance.Application.Commands.Claims.SubmitClaim;
using RadiologyCenter.Insurance.Application.DTOs;
using RadiologyCenter.Insurance.Application.Queries.Claims.GetClaimByExamination;
using RadiologyCenter.Insurance.Application.Queries.Claims.GetClaimById;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Insurance;

[ApiController]
[Route("api/insurance/claims")]
public class ClaimsController : ControllerBase
{
    private readonly IMessageBus _bus;

    public ClaimsController(IMessageBus bus) => _bus = bus;

    [HasPermission(InsuranceClaimsReadCode)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ClaimDto>>(new GetClaimByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(InsuranceClaimsReadCode)]
    [HttpGet("by-examination/{examinationId:guid}")]
    public async Task<IActionResult> GetByExaminationAsync(Guid examinationId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ClaimDto>>(new GetClaimByExaminationQuery(examinationId), ct);
        return result.ToActionResult();
    }

    [HasPermission(InsuranceClaimsCreateCode)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateClaimCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ClaimDto>>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(InsuranceClaimsUpdateCode)]
    [HttpPost("{id:guid}/submit")]
    public async Task<IActionResult> SubmitAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ClaimDto>>(new SubmitClaimCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(InsuranceClaimsUpdateCode)]
    [HttpPost("{id:guid}/adjudicate")]
    public async Task<IActionResult> AdjudicateAsync(Guid id, [FromBody] AdjudicateClaimCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ClaimDto>>(command with { ClaimId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(InsuranceClaimsUpdateCode)]
    [HttpPost("{id:guid}/resubmit")]
    public async Task<IActionResult> ResubmitAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ClaimDto>>(new ResubmitClaimCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(InsuranceClaimsSettleCode)]
    [HttpPost("{id:guid}/settlements")]
    public async Task<IActionResult> RecordSettlementAsync(Guid id, [FromBody] RecordClaimSettlementCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ClaimDto>>(command with { ClaimId = id }, ct);
        return result.ToActionResult();
    }
}