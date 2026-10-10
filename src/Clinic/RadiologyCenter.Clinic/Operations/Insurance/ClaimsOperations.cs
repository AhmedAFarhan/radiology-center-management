using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Insurance.Application.Commands.Claims.AdjudicateClaim;
using RadiologyCenter.Insurance.Application.Commands.Claims.CreateClaim;
using RadiologyCenter.Insurance.Application.Commands.Claims.RecordClaimSettlement;
using RadiologyCenter.Insurance.Application.Commands.Claims.ResubmitClaim;
using RadiologyCenter.Insurance.Application.Commands.Claims.SubmitClaim;
using RadiologyCenter.Insurance.Application.DTOs;
using RadiologyCenter.Insurance.Application.Queries.Claims.GetClaimByExamination;
using RadiologyCenter.Insurance.Application.Queries.Claims.GetClaimById;
using RadiologyCenter.Insurance.Application.Queries.Claims.GetClaims;

using RadiologyCenter.Clinic.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Clinic.Operations.Insurance;

public class ClaimsOperations : ClinicOperations
{
    private readonly IMessageBus _bus;

    public ClaimsOperations(IMessageBus bus) => _bus = bus;

    [ClinicPermission(InsuranceClaimsReadCode)]
    [ClinicOperation("POST", "insurance/claims/all")]
    public async Task<ClinicResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<ClaimListItemDto>>>(new GetClaimsQuery(request), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InsuranceClaimsReadCode)]
    [ClinicOperation("GET", "insurance/claims/{id:guid}")]
    public async Task<ClinicResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ClaimDto>>(new GetClaimByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InsuranceClaimsReadCode)]
    [ClinicOperation("GET", "insurance/claims/by-examination/{examinationId:guid}")]
    public async Task<ClinicResult> GetByExaminationAsync(Guid examinationId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ClaimDto>>(new GetClaimByExaminationQuery(examinationId), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InsuranceClaimsCreateCode)]
    [ClinicOperation("POST", "insurance/claims")]
    public async Task<ClinicResult> CreateAsync([FromBody] CreateClaimCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ClaimDto>>(command, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InsuranceClaimsUpdateCode)]
    [ClinicOperation("POST", "insurance/claims/{id:guid}/submit")]
    public async Task<ClinicResult> SubmitAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ClaimDto>>(new SubmitClaimCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InsuranceClaimsUpdateCode)]
    [ClinicOperation("POST", "insurance/claims/{id:guid}/adjudicate")]
    public async Task<ClinicResult> AdjudicateAsync(Guid id, [FromBody] AdjudicateClaimCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ClaimDto>>(command with { ClaimId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InsuranceClaimsUpdateCode)]
    [ClinicOperation("POST", "insurance/claims/{id:guid}/resubmit")]
    public async Task<ClinicResult> ResubmitAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ClaimDto>>(new ResubmitClaimCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InsuranceClaimsSettleCode)]
    [ClinicOperation("POST", "insurance/claims/{id:guid}/settlements")]
    public async Task<ClinicResult> RecordSettlementAsync(Guid id, [FromBody] RecordClaimSettlementCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ClaimDto>>(command with { ClaimId = id }, ct);
        return result.ToActionResult();
    }
}