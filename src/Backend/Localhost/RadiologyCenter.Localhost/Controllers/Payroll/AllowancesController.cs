using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using RadiologyCenter.Payroll.Application.Commands.ActivateAllowanceAssignment;
using RadiologyCenter.Payroll.Application.Commands.CreateAllowanceAssignment;
using RadiologyCenter.Payroll.Application.Commands.DeactivateAllowanceAssignment;
using RadiologyCenter.Payroll.Application.Commands.DeleteAllowanceAssignment;
using RadiologyCenter.Payroll.Application.Commands.UpdateAllowanceAssignment;
using RadiologyCenter.Payroll.Application.DTOs;
using RadiologyCenter.Payroll.Application.Queries.GetAllowanceAssignmentById;
using RadiologyCenter.Payroll.Application.Queries.GetAllowanceAssignments;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Payroll;

[ApiController]
[Route("api/payroll/allowances")]
public class AllowancesController : ControllerBase
{
    private readonly IMessageBus _bus;

    public AllowancesController(IMessageBus bus) => _bus = bus;

    [HasPermission(PayrollReadCode)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<AllowanceAssignmentDto>>(new GetAllowanceAssignmentByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollReadCode)]
    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<AllowanceAssignmentDto>>>(new GetAllowanceAssignmentsQuery(request), ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollAllowancesManageCode)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateAllowanceAssignmentCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<AllowanceAssignmentDto>>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollAllowancesManageCode)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateAllowanceAssignmentCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { AllowanceAssignmentId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollAllowancesManageCode)]
    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ActivateAllowanceAssignmentCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollAllowancesManageCode)]
    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivateAllowanceAssignmentCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollAllowancesManageCode)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteAllowanceAssignmentCommand(id), ct);
        return result.ToActionResult();
    }
}