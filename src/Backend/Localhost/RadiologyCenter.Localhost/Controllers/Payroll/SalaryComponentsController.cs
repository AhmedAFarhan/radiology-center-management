using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
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

namespace RadiologyCenter.Localhost.Controllers.Payroll;

[ApiController]
[Route("api/payroll/salary-components")]
public class SalaryComponentsController : ControllerBase
{
    private readonly IMessageBus _bus;

    public SalaryComponentsController(IMessageBus bus) => _bus = bus;

    [HasPermission(PayrollReadCode)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<SalaryComponentDto>>(new GetSalaryComponentByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollReadCode)]
    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<SalaryComponentDto>>>(new GetSalaryComponentsQuery(request), ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollSalaryComponentsManageCode)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateSalaryComponentCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<SalaryComponentDto>>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollSalaryComponentsManageCode)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateSalaryComponentCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { SalaryComponentId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollSalaryComponentsManageCode)]
    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ActivateSalaryComponentCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollSalaryComponentsManageCode)]
    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivateSalaryComponentCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollSalaryComponentsManageCode)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteSalaryComponentCommand(id), ct);
        return result.ToActionResult();
    }
}