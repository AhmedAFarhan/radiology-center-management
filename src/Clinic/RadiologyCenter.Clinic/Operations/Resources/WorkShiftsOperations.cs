using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;

using RadiologyCenter.Clinic.Extensions;
using RadiologyCenter.ResourceManagement.Application.Commands.CreateWorkShift;
using RadiologyCenter.ResourceManagement.Application.Commands.DeleteWorkShift;
using RadiologyCenter.ResourceManagement.Application.Commands.UpdateWorkShift;
using RadiologyCenter.ResourceManagement.Application.DTOs;
using RadiologyCenter.ResourceManagement.Application.Queries.GetWorkShiftById;
using RadiologyCenter.ResourceManagement.Application.Queries.GetWorkShifts;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Clinic.Operations.Resources;

public class WorkShiftsOperations : ClinicOperations
{
    private readonly IMessageBus _bus;

    public WorkShiftsOperations(IMessageBus bus) => _bus = bus;

    [ClinicPermission(ShiftsReadCode)]
    [ClinicOperation("GET", "resources/work-shifts/{id:guid}")]
    public async Task<ClinicResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<WorkShiftDto>>(new GetWorkShiftByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ShiftsReadCode)]
    [ClinicOperation("POST", "resources/work-shifts/all")]
    public async Task<ClinicResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<WorkShiftDto>>>(new GetWorkShiftsQuery(request), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ShiftsCreateCode)]
    [ClinicOperation("POST", "resources/work-shifts")]
    public async Task<ClinicResult> CreateAsync([FromBody] CreateWorkShiftCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<WorkShiftDto>>(command, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ShiftsUpdateCode)]
    [ClinicOperation("PUT", "resources/work-shifts/{id:guid}")]
    public async Task<ClinicResult> UpdateAsync(Guid id, [FromBody] UpdateWorkShiftCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { WorkShiftId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ShiftsDeleteCode)]
    [ClinicOperation("DELETE", "resources/work-shifts/{id:guid}")]
    public async Task<ClinicResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteWorkShiftCommand(id), ct);
        return result.ToActionResult();
    }
}
