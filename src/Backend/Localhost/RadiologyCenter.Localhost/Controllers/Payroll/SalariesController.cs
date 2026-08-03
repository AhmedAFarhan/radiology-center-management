using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
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

namespace RadiologyCenter.Localhost.Controllers.Payroll;

[ApiController]
[Route("api/payroll/salaries")]
public class SalariesController : ControllerBase
{
    private readonly IMessageBus _bus;

    public SalariesController(IMessageBus bus) => _bus = bus;

    [HasPermission(PayrollReadCode)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<SalaryDto>>(new GetSalaryByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollReadCode)]
    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<SalaryDto>>>(new GetSalariesQuery(request), ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollSalaryManageCode)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateSalaryCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<SalaryDto>>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollSalaryManageCode)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateSalaryCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { SalaryId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollSalaryManageCode)]
    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ActivateSalaryCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollSalaryManageCode)]
    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivateSalaryCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollSalaryManageCode)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteSalaryCommand(id), ct);
        return result.ToActionResult();
    }
}