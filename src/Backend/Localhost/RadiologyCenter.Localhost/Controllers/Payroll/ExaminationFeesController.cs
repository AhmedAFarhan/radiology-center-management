using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using RadiologyCenter.Payroll.Application.Commands.ActivateExaminationFee;
using RadiologyCenter.Payroll.Application.Commands.CreateExaminationFee;
using RadiologyCenter.Payroll.Application.Commands.DeactivateExaminationFee;
using RadiologyCenter.Payroll.Application.Commands.DeleteExaminationFee;
using RadiologyCenter.Payroll.Application.Commands.UpdateExaminationFee;
using RadiologyCenter.Payroll.Application.DTOs;
using RadiologyCenter.Payroll.Application.Queries.GetExaminationFeeById;
using RadiologyCenter.Payroll.Application.Queries.GetExaminationFees;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Payroll;

[ApiController]
[Route("api/payroll/examination-fees")]
public class ExaminationFeesController : ControllerBase
{
    private readonly IMessageBus _bus;

    public ExaminationFeesController(IMessageBus bus) => _bus = bus;

    [HasPermission(PayrollReadCode)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ExaminationFeeDto>>(new GetExaminationFeeByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollReadCode)]
    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<ExaminationFeeDto>>>(new GetExaminationFeesQuery(request), ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollFeesManageCode)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateExaminationFeeCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ExaminationFeeDto>>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollFeesManageCode)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateExaminationFeeCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { ExaminationFeeId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollFeesManageCode)]
    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ActivateExaminationFeeCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollFeesManageCode)]
    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivateExaminationFeeCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollFeesManageCode)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteExaminationFeeCommand(id), ct);
        return result.ToActionResult();
    }
}