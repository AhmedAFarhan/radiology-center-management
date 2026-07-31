using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Patients.Application.Commands.ActivatePatient;
using RadiologyCenter.Patients.Application.Commands.CreatePatient;
using RadiologyCenter.Patients.Application.Commands.DeactivatePatient;
using RadiologyCenter.Patients.Application.Commands.DeletePatient;
using RadiologyCenter.Patients.Application.Commands.UpdatePatient;
using RadiologyCenter.Patients.Application.DTOs;
using RadiologyCenter.Patients.Application.Queries.GetPatientById;
using RadiologyCenter.Patients.Application.Queries.GetPatients;
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
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<PatientDto>>>(new GetPatientsQuery(request), ct);
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
