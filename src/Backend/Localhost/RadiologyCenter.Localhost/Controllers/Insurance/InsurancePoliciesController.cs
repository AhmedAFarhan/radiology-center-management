using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Insurance.Application.Commands.Policies.ChangePolicyStatus;
using RadiologyCenter.Insurance.Application.Commands.Policies.CreateInsurancePolicy;
using RadiologyCenter.Insurance.Application.Commands.Policies.DeletePolicyDocument;
using RadiologyCenter.Insurance.Application.Commands.Policies.UploadPolicyDocument;
using RadiologyCenter.Insurance.Application.Commands.Policies.UpdateCoverage;
using RadiologyCenter.Insurance.Application.DTOs;
using RadiologyCenter.Insurance.Application.Queries.Policies.GetInsurancePolicies;
using RadiologyCenter.Insurance.Application.Queries.Policies.GetInsurancePolicyById;
using RadiologyCenter.Insurance.Application.Queries.Policies.GetPoliciesByPatient;
using RadiologyCenter.Insurance.Application.Queries.Policies.GetPolicyDocumentContent;
using RadiologyCenter.Insurance.Application.Queries.Policies.GetPolicyDocuments;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Insurance;

[ApiController]
[Route("api/insurance/policies")]
public class InsurancePoliciesController : ControllerBase
{
    private readonly IMessageBus _bus;

    public InsurancePoliciesController(IMessageBus bus) => _bus = bus;

    [HasPermission(InsurancePoliciesReadCode)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<InsurancePolicyDto>>(new GetInsurancePolicyByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(InsurancePoliciesReadCode)]
    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<InsurancePolicyListItemDto>>>(new GetInsurancePoliciesQuery(request), ct);
        return result.ToActionResult();
    }

    [HasPermission(InsurancePoliciesReadCode)]
    [HttpGet("by-patient/{patientId:guid}")]
    public async Task<IActionResult> GetByPatientAsync(Guid patientId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<IReadOnlyList<InsurancePolicyDto>>>(new GetPoliciesByPatientQuery(patientId), ct);
        return result.ToActionResult();
    }

    [HasPermission(InsurancePoliciesCreateCode)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateInsurancePolicyCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<InsurancePolicyDto>>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(InsurancePoliciesUpdateCode)]
    [HttpPut("{id:guid}/coverage")]
    public async Task<IActionResult> UpdateCoverageAsync(Guid id, [FromBody] UpdatePolicyCoverageCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<InsurancePolicyDto>>(command with { PolicyId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(InsurancePoliciesUpdateCode)]
    [HttpPost("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatusAsync(Guid id, [FromBody] ChangePolicyStatusCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<InsurancePolicyDto>>(command with { PolicyId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(InsurancePoliciesReadCode)]
    [HttpGet("{id:guid}/documents")]
    public async Task<IActionResult> GetDocumentsAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<IReadOnlyList<PolicyDocumentDto>>>(new GetPolicyDocumentsQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(InsurancePoliciesReadCode)]
    [HttpGet("{id:guid}/documents/{documentId:guid}/content")]
    public async Task<IActionResult> GetDocumentContentAsync(Guid id, Guid documentId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PolicyDocumentContentDto>>(new GetPolicyDocumentContentQuery(id, documentId), ct);
        if (!result.IsSuccess)
            return result.ToActionResult();
        if (result.Value.Content is null)
            return NotFound();
        return File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
    }

    [HasPermission(InsurancePoliciesReadCode)]
    [HttpPost("{id:guid}/documents")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadDocumentAsync(Guid id, [FromForm] IFormFile file, [FromForm] string type, CancellationToken ct)
    {
        if (file is null)
            return BadRequest(new { Type = "File", Message = "A file is required." });

        await using var content = file.OpenReadStream();
        var command = new UploadPolicyDocumentCommand(id, type, file.FileName, file.ContentType, file.Length, content);
        var result = await _bus.InvokeAsync<Result<PolicyDocumentDto>>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(InsurancePoliciesDeleteCode)]
    [HttpDelete("{id:guid}/documents/{documentId:guid}")]
    public async Task<IActionResult> DeleteDocumentAsync(Guid id, Guid documentId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeletePolicyDocumentCommand(id, documentId), ct);
        return result.ToActionResult();
    }
}