using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Application.Excel;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Catalog.Application.Commands.ActivateExaminationType;
using RadiologyCenter.Catalog.Application.Commands.CreateExaminationType;
using RadiologyCenter.Catalog.Application.Commands.DeactivateExaminationType;
using RadiologyCenter.Catalog.Application.Commands.DeleteExaminationType;
using RadiologyCenter.Catalog.Application.Commands.ImportExaminationTypes;
using RadiologyCenter.Catalog.Application.Commands.UpdateExaminationType;
using RadiologyCenter.Catalog.Application.DTOs;
using RadiologyCenter.Catalog.Application.Queries.ExportExaminationTypes;
using RadiologyCenter.Catalog.Application.Queries.GetExaminationTypeById;
using RadiologyCenter.Catalog.Application.Queries.GetExaminationTypes;
using RadiologyCenter.Catalog.Application.Queries.GetExaminationTypesImportTemplate;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Catalog;

[ApiController]
[Route("api/catalog/examination-types")]
public class ExaminationTypesController : ControllerBase
{
    private readonly IMessageBus _bus;

    public ExaminationTypesController(IMessageBus bus) => _bus = bus;

    [HasPermission(ExaminationsReadCode)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ExaminationTypeDto>>(new GetExaminationTypeByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsReadCode)]
    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct, [FromQuery] bool? isActive = null)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<ExaminationTypeDto>>>(new GetExaminationTypesQuery(request, isActive), ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsReadCode)]
    [HttpPost("export")]
    public async Task<IActionResult> ExportAsync([FromBody] QueryRequest request, CancellationToken ct, [FromQuery] bool? isActive = null)
    {
        var result = await _bus.InvokeAsync<Result<FileContentDto>>(new ExportExaminationTypesQuery(request, isActive), ct);
        if (!result.IsSuccess)
            return result.ToActionResult();
        return File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
    }

    [HasPermission(ExaminationsTypesManageCode)]
    [HttpGet("import-template")]
    public async Task<IActionResult> GetImportTemplateAsync(CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<FileContentDto>>(new GetExaminationTypesImportTemplateQuery(), ct);
        if (!result.IsSuccess)
            return result.ToActionResult();
        return File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
    }

    [HasPermission(ExaminationsTypesManageCode)]
    [HttpPost("import")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ImportAsync([FromForm] ImportExcelForm form, CancellationToken ct)
    {
        if (form.File is null || form.File.Length == 0)
            return BadRequest(new { Type = "File", Message = "An Excel file is required." });

        using var stream = new MemoryStream();
        await form.File.CopyToAsync(stream, ct);
        var result = await _bus.InvokeAsync<Result<ExcelImportResult>>(new ImportExaminationTypesCommand(stream.ToArray()), ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsTypesManageCode)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateExaminationTypeCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ExaminationTypeDto>>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsTypesManageCode)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateExaminationTypeCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { ExaminationTypeId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsTypesManageCode)]
    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ActivateExaminationTypeCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsTypesManageCode)]
    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivateExaminationTypeCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsTypesManageCode)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteExaminationTypeCommand(id), ct);
        return result.ToActionResult();
    }
}

public sealed class ImportExcelForm
{
    public IFormFile File { get; set; } = null!;
}