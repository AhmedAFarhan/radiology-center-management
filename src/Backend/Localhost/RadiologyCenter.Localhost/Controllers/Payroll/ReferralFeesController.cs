using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using RadiologyCenter.Payroll.Application.Commands.ActivateReferralFee;
using RadiologyCenter.Payroll.Application.Commands.CreateReferralFee;
using RadiologyCenter.Payroll.Application.Commands.DeactivateReferralFee;
using RadiologyCenter.Payroll.Application.Commands.DeleteReferralFee;
using RadiologyCenter.Payroll.Application.Commands.UpdateReferralFee;
using RadiologyCenter.Payroll.Application.DTOs;
using RadiologyCenter.Payroll.Application.Queries.GetReferralFeeById;
using RadiologyCenter.Payroll.Application.Queries.GetReferralFees;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Payroll;

[ApiController]
[Route("api/payroll/referral-fees")]
public class ReferralFeesController : ControllerBase
{
    private readonly IMessageBus _bus;

    public ReferralFeesController(IMessageBus bus) => _bus = bus;

    [HasPermission(PayrollReadCode)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReferralFeeDto>>(new GetReferralFeeByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollReadCode)]
    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<ReferralFeeDto>>>(new GetReferralFeesQuery(request), ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollFeesManageCode)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateReferralFeeCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReferralFeeDto>>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollFeesManageCode)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateReferralFeeCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { Id = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollFeesManageCode)]
    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ActivateReferralFeeCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollFeesManageCode)]
    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivateReferralFeeCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollFeesManageCode)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteReferralFeeCommand(id), ct);
        return result.ToActionResult();
    }
}