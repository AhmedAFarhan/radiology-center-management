using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;

using RadiologyCenter.Clinic.Extensions;
using RadiologyCenter.Payroll.Application.Commands.CreateSalaryComponent;
using RadiologyCenter.Payroll.Application.Commands.UpdateSalaryComponent;
using RadiologyCenter.Payroll.Application.Commands.ActivateSalaryComponent;
using RadiologyCenter.Payroll.Application.Commands.DeactivateSalaryComponent;
using RadiologyCenter.Payroll.Application.Commands.DeleteSalaryComponent;
using RadiologyCenter.Payroll.Application.DTOs;
using RadiologyCenter.Payroll.Application.Queries.GetSalaryComponentById;
using RadiologyCenter.Payroll.Application.Queries.GetSalaryComponents;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Clinic.Operations.Payroll;

public class SalaryComponentsOperations : ClinicOperations
{
    private readonly IMessageBus _bus;

    public SalaryComponentsOperations(IMessageBus bus) => _bus = bus;

    [ClinicPermission(PayrollReadCode)]
    [ClinicOperation("GET", "payroll/salary-components/{id:guid}")]
    public async Task<ClinicResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<SalaryComponentDto>>(new GetSalaryComponentByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PayrollReadCode)]
    [ClinicOperation("POST", "payroll/salary-components/all")]
    public async Task<ClinicResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<SalaryComponentDto>>>(new GetSalaryComponentsQuery(request), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PayrollSalaryComponentsManageCode)]
    [ClinicOperation("POST", "payroll/salary-components")]
    public async Task<ClinicResult> CreateAsync([FromBody] CreateSalaryComponentCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<SalaryComponentDto>>(command, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PayrollSalaryComponentsManageCode)]
    [ClinicOperation("PUT", "payroll/salary-components/{id:guid}")]
    public async Task<ClinicResult> UpdateAsync(Guid id, [FromBody] UpdateSalaryComponentCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { SalaryComponentId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PayrollSalaryComponentsManageCode)]
    [ClinicOperation("POST", "payroll/salary-components/{id:guid}/activate")]
    public async Task<ClinicResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ActivateSalaryComponentCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PayrollSalaryComponentsManageCode)]
    [ClinicOperation("POST", "payroll/salary-components/{id:guid}/deactivate")]
    public async Task<ClinicResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivateSalaryComponentCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PayrollSalaryComponentsManageCode)]
    [ClinicOperation("DELETE", "payroll/salary-components/{id:guid}")]
    public async Task<ClinicResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteSalaryComponentCommand(id), ct);
        return result.ToActionResult();
    }
}