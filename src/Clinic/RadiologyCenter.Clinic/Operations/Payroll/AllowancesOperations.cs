using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;

using RadiologyCenter.Clinic.Extensions;
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

namespace RadiologyCenter.Clinic.Operations.Payroll;

public class AllowancesOperations : ClinicOperations
{
    private readonly IMessageBus _bus;

    public AllowancesOperations(IMessageBus bus) => _bus = bus;

    [ClinicPermission(PayrollReadCode)]
    [ClinicOperation("GET", "payroll/allowances/{id:guid}")]
    public async Task<ClinicResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<AllowanceAssignmentDto>>(new GetAllowanceAssignmentByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PayrollReadCode)]
    [ClinicOperation("POST", "payroll/allowances/all")]
    public async Task<ClinicResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<AllowanceAssignmentDto>>>(new GetAllowanceAssignmentsQuery(request), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PayrollAllowancesManageCode)]
    [ClinicOperation("POST", "payroll/allowances")]
    public async Task<ClinicResult> CreateAsync([FromBody] CreateAllowanceAssignmentCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<AllowanceAssignmentDto>>(command, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PayrollAllowancesManageCode)]
    [ClinicOperation("PUT", "payroll/allowances/{id:guid}")]
    public async Task<ClinicResult> UpdateAsync(Guid id, [FromBody] UpdateAllowanceAssignmentCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { AllowanceAssignmentId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PayrollAllowancesManageCode)]
    [ClinicOperation("POST", "payroll/allowances/{id:guid}/activate")]
    public async Task<ClinicResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ActivateAllowanceAssignmentCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PayrollAllowancesManageCode)]
    [ClinicOperation("POST", "payroll/allowances/{id:guid}/deactivate")]
    public async Task<ClinicResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivateAllowanceAssignmentCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PayrollAllowancesManageCode)]
    [ClinicOperation("DELETE", "payroll/allowances/{id:guid}")]
    public async Task<ClinicResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteAllowanceAssignmentCommand(id), ct);
        return result.ToActionResult();
    }
}