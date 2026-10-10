using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;

using RadiologyCenter.Clinic.Extensions;
using RadiologyCenter.ResourceManagement.Application.Commands.ActivateStaff;
using RadiologyCenter.ResourceManagement.Application.Commands.CreateStaff;
using RadiologyCenter.ResourceManagement.Application.Commands.DeactivateStaff;
using RadiologyCenter.ResourceManagement.Application.Commands.DeleteStaff;
using RadiologyCenter.ResourceManagement.Application.Commands.UpdateStaff;
using RadiologyCenter.ResourceManagement.Application.DTOs;
using RadiologyCenter.ResourceManagement.Application.Queries.GetStaffById;
using RadiologyCenter.ResourceManagement.Application.Queries.GetStaffs;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Clinic.Operations.Resources;

public class StaffOperations : ClinicOperations
{
    private readonly IMessageBus _bus;

    public StaffOperations(IMessageBus bus) => _bus = bus;

    [ClinicPermission(StaffReadCode)]
    [ClinicOperation("GET", "resources/staff/{id:guid}")]
    public async Task<ClinicResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<StaffDto>>(new GetStaffByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(StaffReadCode)]
    [ClinicOperation("POST", "resources/staff/all")]
    public async Task<ClinicResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<StaffDto>>>(new GetStaffsQuery(request), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(StaffCreateCode)]
    [ClinicOperation("POST", "resources/staff")]
    public async Task<ClinicResult> CreateAsync([FromBody] CreateStaffCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<StaffDto>>(command, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(StaffUpdateCode)]
    [ClinicOperation("PUT", "resources/staff/{id:guid}")]
    public async Task<ClinicResult> UpdateAsync(Guid id, [FromBody] UpdateStaffCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { StaffId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(StaffUpdateCode)]
    [ClinicOperation("POST", "resources/staff/{id:guid}/activate")]
    public async Task<ClinicResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ActivateStaffCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(StaffUpdateCode)]
    [ClinicOperation("POST", "resources/staff/{id:guid}/deactivate")]
    public async Task<ClinicResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivateStaffCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(StaffDeleteCode)]
    [ClinicOperation("DELETE", "resources/staff/{id:guid}")]
    public async Task<ClinicResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteStaffCommand(id), ct);
        return result.ToActionResult();
    }
}
