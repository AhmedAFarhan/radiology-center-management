using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;

using RadiologyCenter.Clinic.Extensions;
using RadiologyCenter.ResourceManagement.Application.Commands.CreateLeave;
using RadiologyCenter.ResourceManagement.Application.Commands.DeleteLeave;
using RadiologyCenter.ResourceManagement.Application.Commands.UpdateLeave;
using RadiologyCenter.ResourceManagement.Application.DTOs;
using RadiologyCenter.ResourceManagement.Application.Queries.GetLeaveById;
using RadiologyCenter.ResourceManagement.Application.Queries.GetLeaves;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Clinic.Operations.Resources;

public class LeaveOperations : ClinicOperations
{
    private readonly IMessageBus _bus;

    public LeaveOperations(IMessageBus bus) => _bus = bus;

    [ClinicPermission(LeaveReadCode)]
    [ClinicOperation("GET", "resources/leaves/{id:guid}")]
    public async Task<ClinicResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<LeaveDto>>(new GetLeaveByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(LeaveReadCode)]
    [ClinicOperation("POST", "resources/leaves/all")]
    public async Task<ClinicResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<LeaveDto>>>(new GetLeavesQuery(request), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(LeaveCreateCode)]
    [ClinicOperation("POST", "resources/leaves")]
    public async Task<ClinicResult> CreateAsync([FromBody] CreateLeaveCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<LeaveDto>>(command, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(LeaveUpdateCode)]
    [ClinicOperation("PUT", "resources/leaves/{id:guid}")]
    public async Task<ClinicResult> UpdateAsync(Guid id, [FromBody] UpdateLeaveCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { LeaveId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(LeaveDeleteCode)]
    [ClinicOperation("DELETE", "resources/leaves/{id:guid}")]
    public async Task<ClinicResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteLeaveCommand(id), ct);
        return result.ToActionResult();
    }
}
