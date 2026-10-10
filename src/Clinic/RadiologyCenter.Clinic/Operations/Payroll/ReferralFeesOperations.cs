using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;

using RadiologyCenter.Clinic.Extensions;
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

namespace RadiologyCenter.Clinic.Operations.Payroll;

public class ReferralFeesOperations : ClinicOperations
{
    private readonly IMessageBus _bus;

    public ReferralFeesOperations(IMessageBus bus) => _bus = bus;

    [ClinicPermission(PayrollReadCode)]
    [ClinicOperation("GET", "payroll/referral-fees/{id:guid}")]
    public async Task<ClinicResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReferralFeeDto>>(new GetReferralFeeByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PayrollReadCode)]
    [ClinicOperation("POST", "payroll/referral-fees/all")]
    public async Task<ClinicResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<ReferralFeeDto>>>(new GetReferralFeesQuery(request), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PayrollFeesManageCode)]
    [ClinicOperation("POST", "payroll/referral-fees")]
    public async Task<ClinicResult> CreateAsync([FromBody] CreateReferralFeeCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReferralFeeDto>>(command, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PayrollFeesManageCode)]
    [ClinicOperation("PUT", "payroll/referral-fees/{id:guid}")]
    public async Task<ClinicResult> UpdateAsync(Guid id, [FromBody] UpdateReferralFeeCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { Id = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PayrollFeesManageCode)]
    [ClinicOperation("POST", "payroll/referral-fees/{id:guid}/activate")]
    public async Task<ClinicResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ActivateReferralFeeCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PayrollFeesManageCode)]
    [ClinicOperation("POST", "payroll/referral-fees/{id:guid}/deactivate")]
    public async Task<ClinicResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivateReferralFeeCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PayrollFeesManageCode)]
    [ClinicOperation("DELETE", "payroll/referral-fees/{id:guid}")]
    public async Task<ClinicResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteReferralFeeCommand(id), ct);
        return result.ToActionResult();
    }
}