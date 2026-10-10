using RadiologyCenter.Clinic.Dispatching;
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

using RadiologyCenter.Clinic.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Clinic.Operations.Insurance;

public class InsurancePoliciesOperations : ClinicOperations
{
    private readonly IMessageBus _bus;

    public InsurancePoliciesOperations(IMessageBus bus) => _bus = bus;

    [ClinicPermission(InsurancePoliciesReadCode)]
    [ClinicOperation("GET", "insurance/policies/{id:guid}")]
    public async Task<ClinicResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<InsurancePolicyDto>>(new GetInsurancePolicyByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InsurancePoliciesReadCode)]
    [ClinicOperation("POST", "insurance/policies/all")]
    public async Task<ClinicResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<InsurancePolicyListItemDto>>>(new GetInsurancePoliciesQuery(request), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InsurancePoliciesReadCode)]
    [ClinicOperation("GET", "insurance/policies/by-patient/{patientId:guid}")]
    public async Task<ClinicResult> GetByPatientAsync(Guid patientId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<IReadOnlyList<InsurancePolicyDto>>>(new GetPoliciesByPatientQuery(patientId), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InsurancePoliciesCreateCode)]
    [ClinicOperation("POST", "insurance/policies")]
    public async Task<ClinicResult> CreateAsync([FromBody] CreateInsurancePolicyCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<InsurancePolicyDto>>(command, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InsurancePoliciesUpdateCode)]
    [ClinicOperation("PUT", "insurance/policies/{id:guid}/coverage")]
    public async Task<ClinicResult> UpdateCoverageAsync(Guid id, [FromBody] UpdatePolicyCoverageCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<InsurancePolicyDto>>(command with { PolicyId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InsurancePoliciesUpdateCode)]
    [ClinicOperation("POST", "insurance/policies/{id:guid}/status")]
    public async Task<ClinicResult> ChangeStatusAsync(Guid id, [FromBody] ChangePolicyStatusCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<InsurancePolicyDto>>(command with { PolicyId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InsurancePoliciesReadCode)]
    [ClinicOperation("GET", "insurance/policies/{id:guid}/documents")]
    public async Task<ClinicResult> GetDocumentsAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<IReadOnlyList<PolicyDocumentDto>>>(new GetPolicyDocumentsQuery(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InsurancePoliciesReadCode)]
    [ClinicOperation("GET", "insurance/policies/{id:guid}/documents/{documentId:guid}/content")]
    public async Task<ClinicResult> GetDocumentContentAsync(Guid id, Guid documentId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PolicyDocumentContentDto>>(new GetPolicyDocumentContentQuery(id, documentId), ct);
        if (!result.IsSuccess)
            return result.ToActionResult();
        if (result.Value.Content is null)
            return NotFound();
        return File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
    }

    [ClinicPermission(InsurancePoliciesUpdateCode)]
    [ClinicOperation("POST", "insurance/policies/{id:guid}/documents")]

    public async Task<ClinicResult> UploadDocumentAsync(Guid id, [FromForm] UploadPolicyDocumentForm form, CancellationToken ct)
    {
        if (form.File is null)
            return BadRequest(new { Type = "File", Message = "A file is required." });

        await using var content = form.File.OpenReadStream();
        var command = new UploadPolicyDocumentCommand(id, form.Type, form.File.FileName, form.File.ContentType, form.File.Length, content);
        var result = await _bus.InvokeAsync<Result<PolicyDocumentDto>>(command, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InsurancePoliciesDeleteCode)]
    [ClinicOperation("DELETE", "insurance/policies/{id:guid}/documents/{documentId:guid}")]
    public async Task<ClinicResult> DeleteDocumentAsync(Guid id, Guid documentId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeletePolicyDocumentCommand(id, documentId), ct);
        return result.ToActionResult();
    }
}

public sealed class UploadPolicyDocumentForm
{
    public IFormFile File { get; set; } = null!;

    public string Type { get; set; } = string.Empty;
}