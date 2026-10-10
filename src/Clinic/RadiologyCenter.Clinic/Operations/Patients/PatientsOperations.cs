using RadiologyCenter.Clinic.Dispatching;
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

using RadiologyCenter.Clinic.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Clinic.Operations.Patients;

public class PatientsOperations : ClinicOperations
{
    private readonly IMessageBus _bus;

    public PatientsOperations(IMessageBus bus) => _bus = bus;

    [ClinicPermission(PatientsReadCode)]
    [ClinicOperation("GET", "patients/{id:guid}")]
    public async Task<ClinicResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PatientDto>>(new GetPatientByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PatientsReadCode)]
    [ClinicOperation("POST", "patients/all")]
    public async Task<ClinicResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<PatientDto>>>(new GetPatientsQuery(request), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PatientsReadCode)]
    [ClinicOperation("POST", "patients/export")]
    public async Task<ClinicResult> ExportAsync([FromBody] QueryRequest request, CancellationToken ct, [FromQuery] bool? isActive = null)
    {
        var result = await _bus.InvokeAsync<Result<FileContentDto>>(new ExportPatientsQuery(request, isActive), ct);
        if (!result.IsSuccess)
            return result.ToActionResult();
        return File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
    }

    [ClinicPermission(PatientsCreateCode)]
    [ClinicOperation("GET", "patients/import-template")]
    public async Task<ClinicResult> GetImportTemplateAsync(CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<FileContentDto>>(new GetPatientsImportTemplateQuery(), ct);
        if (!result.IsSuccess)
            return result.ToActionResult();
        return File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
    }

    [ClinicPermission(PatientsCreateCode)]
    [ClinicOperation("POST", "patients/import")]

    public async Task<ClinicResult> ImportAsync([FromForm] ImportExcelForm form, CancellationToken ct)
    {
        if (form.File is null || form.File.Length == 0)
            return BadRequest(new { Type = "File", Message = "An Excel file is required." });

        using var stream = new MemoryStream();
        await form.File.CopyToAsync(stream, ct);
        var result = await _bus.InvokeAsync<Result<ExcelImportResult>>(new ImportPatientsCommand(stream.ToArray()), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PatientsCreateCode)]
    [ClinicOperation("POST", "patients")]
    public async Task<ClinicResult> CreateAsync([FromBody] CreatePatientCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PatientDto>>(command, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PatientsUpdateCode)]
    [ClinicOperation("PUT", "patients/{id:guid}")]
    public async Task<ClinicResult> UpdateAsync(Guid id, [FromBody] UpdatePatientCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { PatientId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PatientsUpdateCode)]
    [ClinicOperation("POST", "patients/{id:guid}/activate")]
    public async Task<ClinicResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ActivatePatientCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PatientsUpdateCode)]
    [ClinicOperation("POST", "patients/{id:guid}/deactivate")]
    public async Task<ClinicResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivatePatientCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PatientsDeleteCode)]
    [ClinicOperation("DELETE", "patients/{id:guid}")]
    public async Task<ClinicResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeletePatientCommand(id), ct);
        return result.ToActionResult();
    }
}

public sealed class ImportExcelForm
{
    public IFormFile File { get; set; } = null!;
}
