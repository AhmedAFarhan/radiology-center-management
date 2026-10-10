using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Insurance.Application.Commands.PreAuthorizations.CreatePreAuthorization;
using RadiologyCenter.Insurance.Application.Commands.PreAuthorizations.DecidePreAuthorization;
using RadiologyCenter.Insurance.Application.Commands.PreAuthorizations.DeletePreAuthorizationDocument;
using RadiologyCenter.Insurance.Application.Commands.PreAuthorizations.UploadPreAuthorizationDocument;
using RadiologyCenter.Insurance.Application.DTOs;
using RadiologyCenter.Insurance.Application.Queries.PreAuthorizations.GetPreAuthorizationByExamination;
using RadiologyCenter.Insurance.Application.Queries.PreAuthorizations.GetPreAuthorizationDocumentContent;
using RadiologyCenter.Insurance.Application.Queries.PreAuthorizations.GetPreAuthorizationDocuments;
using RadiologyCenter.Insurance.Application.Queries.PreAuthorizations.GetPreAuthorizations;

using RadiologyCenter.Clinic.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Clinic.Operations.Insurance;

public class PreAuthorizationsOperations : ClinicOperations
{
    private readonly IMessageBus _bus;

    public PreAuthorizationsOperations(IMessageBus bus) => _bus = bus;

    [ClinicPermission(InsurancePreAuthorizationsReadCode)]
    [ClinicOperation("POST", "insurance/preauthorizations/all")]
    public async Task<ClinicResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<PreAuthorizationListItemDto>>>(new GetPreAuthorizationsQuery(request), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InsurancePreAuthorizationsReadCode)]
    [ClinicOperation("GET", "insurance/preauthorizations/by-examination/{examinationId:guid}")]
    public async Task<ClinicResult> GetByExaminationAsync(Guid examinationId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PreAuthorizationDto>>(new GetPreAuthorizationByExaminationQuery(examinationId), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InsurancePreAuthorizationsCreateCode)]
    [ClinicOperation("POST", "insurance/preauthorizations")]
    public async Task<ClinicResult> CreateAsync([FromBody] CreatePreAuthorizationCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PreAuthorizationDto>>(command, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InsurancePreAuthorizationsUpdateCode)]
    [ClinicOperation("POST", "insurance/preauthorizations/{id:guid}/decide")]
    public async Task<ClinicResult> DecideAsync(Guid id, [FromBody] DecidePreAuthorizationCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PreAuthorizationDto>>(command with { PreAuthorizationId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InsurancePreAuthorizationsReadCode)]
    [ClinicOperation("GET", "insurance/preauthorizations/{id:guid}/documents")]
    public async Task<ClinicResult> GetDocumentsAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<IReadOnlyList<PreAuthorizationDocumentDto>>>(new GetPreAuthorizationDocumentsQuery(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InsurancePreAuthorizationsReadCode)]
    [ClinicOperation("GET", "insurance/preauthorizations/{id:guid}/documents/{documentId:guid}/content")]
    public async Task<ClinicResult> GetDocumentContentAsync(Guid id, Guid documentId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PreAuthorizationDocumentContentDto>>(new GetPreAuthorizationDocumentContentQuery(id, documentId), ct);
        if (!result.IsSuccess)
            return result.ToActionResult();
        if (result.Value.Content is null)
            return NotFound();
        return File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
    }

    [ClinicPermission(InsurancePreAuthorizationsAttachDocumentCode)]
    [ClinicOperation("POST", "insurance/preauthorizations/{id:guid}/documents")]

    public async Task<ClinicResult> UploadDocumentAsync(Guid id, [FromForm] UploadPreAuthorizationDocumentForm form, CancellationToken ct)
    {
        if (form.File is null)
            return BadRequest(new { Type = "File", Message = "A file is required." });

        await using var content = form.File.OpenReadStream();
        var command = new UploadPreAuthorizationDocumentCommand(id, form.Type, form.File.FileName, form.File.ContentType, form.File.Length, content);
        var result = await _bus.InvokeAsync<Result<PreAuthorizationDocumentDto>>(command, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InsurancePreAuthorizationsAttachDocumentCode)]
    [ClinicOperation("DELETE", "insurance/preauthorizations/{id:guid}/documents/{documentId:guid}")]
    public async Task<ClinicResult> DeleteDocumentAsync(Guid id, Guid documentId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeletePreAuthorizationDocumentCommand(id, documentId), ct);
        return result.ToActionResult();
    }
}

public sealed class UploadPreAuthorizationDocumentForm
{
    public IFormFile File { get; set; } = null!;

    public string Type { get; set; } = string.Empty;
}