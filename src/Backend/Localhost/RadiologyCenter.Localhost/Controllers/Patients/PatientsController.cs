using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Application.Excel;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Patients.Application.Commands.ActivatePatient;
using RadiologyCenter.Patients.Application.Commands.CreatePatient;
using RadiologyCenter.Patients.Application.Commands.DeactivatePatient;
using RadiologyCenter.Patients.Application.Commands.DeletePatient;
using RadiologyCenter.Patients.Application.Commands.ImportPatients;
using RadiologyCenter.Patients.Application.Commands.UpdatePatient;
using RadiologyCenter.Patients.Application.DTOs;
using RadiologyCenter.Patients.Application.Queries.ExportPatients;
using RadiologyCenter.Patients.Application.Queries.GetPatientById;
using RadiologyCenter.Patients.Application.Queries.GetPatients;
using RadiologyCenter.Patients.Application.Queries.GetPatientsImportTemplate;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Patients;

[ApiController]
[Route("api/patients")]
public class PatientsController : ControllerBase
{
    private readonly IMessageBus _bus;

    public PatientsController(IMessageBus bus) => _bus = bus;

    [HasPermission(PatientsReadCode)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PatientDto>>(new GetPatientByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(PatientsReadCode)]
    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct, [FromQuery] bool? isActive = null)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<PatientDto>>>(new GetPatientsQuery(request, isActive), ct);
        return result.ToActionResult();
    }

    [HasPermission(PatientsReadCode)]
    [HttpPost("export")]
    public async Task<IActionResult> ExportAsync([FromBody] QueryRequest request, CancellationToken ct, [FromQuery] bool? isActive = null)
    {
        var result = await _bus.InvokeAsync<Result<FileContentDto>>(new ExportPatientsQuery(request, isActive), ct);
        if (!result.IsSuccess)
            return result.ToActionResult();
        return File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
    }

    [HasPermission(PatientsCreateCode)]
    [HttpGet("import-template")]
    public async Task<IActionResult> GetImportTemplateAsync(CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<FileContentDto>>(new GetPatientsImportTemplateQuery(), ct);
        if (!result.IsSuccess)
            return result.ToActionResult();
        return File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
    }

    [HasPermission(PatientsCreateCode)]
    [HttpPost("import")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ImportAsync([FromForm] ImportExcelForm form, CancellationToken ct)
    {
        if (form.File is null || form.File.Length == 0)
            return BadRequest(new { Type = "File", Message = "An Excel file is required." });

        using var stream = new MemoryStream();
        await form.File.CopyToAsync(stream, ct);
        var result = await _bus.InvokeAsync<Result<ExcelImportResult>>(new ImportPatientsCommand(stream.ToArray()), ct);
        return result.ToActionResult();
    }

    [HasPermission(PatientsCreateCode)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreatePatientCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PatientDto>>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(PatientsUpdateCode)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdatePatientCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { PatientId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(PatientsUpdateCode)]
    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ActivatePatientCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(PatientsUpdateCode)]
    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivatePatientCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(PatientsDeleteCode)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeletePatientCommand(id), ct);
        return result.ToActionResult();
    }
}

public sealed class ImportExcelForm
{
    public IFormFile File { get; set; } = null!;
}
