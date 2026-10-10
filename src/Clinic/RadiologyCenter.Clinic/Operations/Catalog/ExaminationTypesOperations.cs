using RadiologyCenter.Clinic.Dispatching;
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

using RadiologyCenter.Clinic.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Clinic.Operations.Catalog;

public class ExaminationTypesOperations : ClinicOperations
{
    private readonly IMessageBus _bus;

    public ExaminationTypesOperations(IMessageBus bus) => _bus = bus;

    [ClinicPermission(ExaminationsReadCode)]
    [ClinicOperation("GET", "catalog/examination-types/{id:guid}")]
    public async Task<ClinicResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ExaminationTypeDto>>(new GetExaminationTypeByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ExaminationsReadCode)]
    [ClinicOperation("POST", "catalog/examination-types/all")]
    public async Task<ClinicResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<ExaminationTypeDto>>>(new GetExaminationTypesQuery(request), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ExaminationsReadCode)]
    [ClinicOperation("POST", "catalog/examination-types/export")]
    public async Task<ClinicResult> ExportAsync([FromBody] QueryRequest request, CancellationToken ct, [FromQuery] bool? isActive = null)
    {
        var result = await _bus.InvokeAsync<Result<FileContentDto>>(new ExportExaminationTypesQuery(request, isActive), ct);
        if (!result.IsSuccess)
            return result.ToActionResult();
        return File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
    }

    [ClinicPermission(ExaminationsTypesManageCode)]
    [ClinicOperation("GET", "catalog/examination-types/import-template")]
    public async Task<ClinicResult> GetImportTemplateAsync(CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<FileContentDto>>(new GetExaminationTypesImportTemplateQuery(), ct);
        if (!result.IsSuccess)
            return result.ToActionResult();
        return File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
    }

    [ClinicPermission(ExaminationsTypesManageCode)]
    [ClinicOperation("POST", "catalog/examination-types/import")]

    public async Task<ClinicResult> ImportAsync([FromForm] ImportExcelForm form, CancellationToken ct)
    {
        if (form.File is null || form.File.Length == 0)
            return BadRequest(new { Type = "File", Message = "An Excel file is required." });

        using var stream = new MemoryStream();
        await form.File.CopyToAsync(stream, ct);
        var result = await _bus.InvokeAsync<Result<ExcelImportResult>>(new ImportExaminationTypesCommand(stream.ToArray()), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ExaminationsTypesManageCode)]
    [ClinicOperation("POST", "catalog/examination-types")]
    public async Task<ClinicResult> CreateAsync([FromBody] CreateExaminationTypeCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ExaminationTypeDto>>(command, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ExaminationsTypesManageCode)]
    [ClinicOperation("PUT", "catalog/examination-types/{id:guid}")]
    public async Task<ClinicResult> UpdateAsync(Guid id, [FromBody] UpdateExaminationTypeCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { ExaminationTypeId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ExaminationsTypesManageCode)]
    [ClinicOperation("POST", "catalog/examination-types/{id:guid}/activate")]
    public async Task<ClinicResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ActivateExaminationTypeCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ExaminationsTypesManageCode)]
    [ClinicOperation("POST", "catalog/examination-types/{id:guid}/deactivate")]
    public async Task<ClinicResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivateExaminationTypeCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ExaminationsTypesManageCode)]
    [ClinicOperation("DELETE", "catalog/examination-types/{id:guid}")]
    public async Task<ClinicResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteExaminationTypeCommand(id), ct);
        return result.ToActionResult();
    }
}

public sealed class ImportExcelForm
{
    public IFormFile File { get; set; } = null!;
}