using Microsoft.AspNetCore.Mvc;
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
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Insurance;

[ApiController]
[Route("api/insurance/preauthorizations")]
public class PreAuthorizationsController : ControllerBase
{
    private readonly IMessageBus _bus;

    public PreAuthorizationsController(IMessageBus bus) => _bus = bus;

    [HasPermission(InsurancePreAuthorizationsReadCode)]
    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<PreAuthorizationListItemDto>>>(new GetPreAuthorizationsQuery(request), ct);
        return result.ToActionResult();
    }

    [HasPermission(InsurancePreAuthorizationsReadCode)]
    [HttpGet("by-examination/{examinationId:guid}")]
    public async Task<IActionResult> GetByExaminationAsync(Guid examinationId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PreAuthorizationDto>>(new GetPreAuthorizationByExaminationQuery(examinationId), ct);
        return result.ToActionResult();
    }

    [HasPermission(InsurancePreAuthorizationsCreateCode)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreatePreAuthorizationCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PreAuthorizationDto>>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(InsurancePreAuthorizationsUpdateCode)]
    [HttpPost("{id:guid}/decide")]
    public async Task<IActionResult> DecideAsync(Guid id, [FromBody] DecidePreAuthorizationCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PreAuthorizationDto>>(command with { PreAuthorizationId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(InsurancePreAuthorizationsReadCode)]
    [HttpGet("{id:guid}/documents")]
    public async Task<IActionResult> GetDocumentsAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<IReadOnlyList<PreAuthorizationDocumentDto>>>(new GetPreAuthorizationDocumentsQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(InsurancePreAuthorizationsReadCode)]
    [HttpGet("{id:guid}/documents/{documentId:guid}/content")]
    public async Task<IActionResult> GetDocumentContentAsync(Guid id, Guid documentId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PreAuthorizationDocumentContentDto>>(new GetPreAuthorizationDocumentContentQuery(id, documentId), ct);
        if (!result.IsSuccess)
            return result.ToActionResult();
        if (result.Value.Content is null)
            return NotFound();
        return File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
    }

    [HasPermission(InsurancePreAuthorizationsAttachDocumentCode)]
    [HttpPost("{id:guid}/documents")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadDocumentAsync(Guid id, [FromForm] IFormFile file, [FromForm] string type, CancellationToken ct)
    {
        if (file is null)
            return BadRequest(new { Type = "File", Message = "A file is required." });

        await using var content = file.OpenReadStream();
        var command = new UploadPreAuthorizationDocumentCommand(id, type, file.FileName, file.ContentType, file.Length, content);
        var result = await _bus.InvokeAsync<Result<PreAuthorizationDocumentDto>>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(InsurancePreAuthorizationsAttachDocumentCode)]
    [HttpDelete("{id:guid}/documents/{documentId:guid}")]
    public async Task<IActionResult> DeleteDocumentAsync(Guid id, Guid documentId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeletePreAuthorizationDocumentCommand(id, documentId), ct);
        return result.ToActionResult();
    }
}