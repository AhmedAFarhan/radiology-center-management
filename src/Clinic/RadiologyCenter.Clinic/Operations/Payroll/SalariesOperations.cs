using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;

using RadiologyCenter.Clinic.Extensions;
using RadiologyCenter.Payroll.Application.Commands.ActivateSalary;
using RadiologyCenter.Payroll.Application.Commands.CreateSalary;
using RadiologyCenter.Payroll.Application.Commands.DeactivateSalary;
using RadiologyCenter.Payroll.Application.Commands.DeleteSalary;
using RadiologyCenter.Payroll.Application.Commands.UpdateSalary;
using RadiologyCenter.Payroll.Application.DTOs;
using RadiologyCenter.Payroll.Application.Queries.GetSalaries;
using RadiologyCenter.Payroll.Application.Queries.GetSalaryById;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Clinic.Operations.Payroll;

public class SalariesOperations : ClinicOperations
{
    private readonly IMessageBus _bus;

    public SalariesOperations(IMessageBus bus) => _bus = bus;

    [ClinicPermission(PayrollReadCode)]
    [ClinicOperation("GET", "payroll/salaries/{id:guid}")]
    public async Task<ClinicResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<SalaryDto>>(new GetSalaryByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PayrollReadCode)]
    [ClinicOperation("POST", "payroll/salaries/all")]
    public async Task<ClinicResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<SalaryDto>>>(new GetSalariesQuery(request), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PayrollSalaryManageCode)]
    [ClinicOperation("POST", "payroll/salaries")]
    public async Task<ClinicResult> CreateAsync([FromBody] CreateSalaryCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<SalaryDto>>(command, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PayrollSalaryManageCode)]
    [ClinicOperation("PUT", "payroll/salaries/{id:guid}")]
    public async Task<ClinicResult> UpdateAsync(Guid id, [FromBody] UpdateSalaryCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { SalaryId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PayrollSalaryManageCode)]
    [ClinicOperation("POST", "payroll/salaries/{id:guid}/activate")]
    public async Task<ClinicResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ActivateSalaryCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PayrollSalaryManageCode)]
    [ClinicOperation("POST", "payroll/salaries/{id:guid}/deactivate")]
    public async Task<ClinicResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivateSalaryCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PayrollSalaryManageCode)]
    [ClinicOperation("DELETE", "payroll/salaries/{id:guid}")]
    public async Task<ClinicResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteSalaryCommand(id), ct);
        return result.ToActionResult();
    }
}