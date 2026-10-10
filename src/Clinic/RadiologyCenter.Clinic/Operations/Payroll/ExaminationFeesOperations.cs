using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;

using RadiologyCenter.Clinic.Extensions;
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

namespace RadiologyCenter.Clinic.Operations.Payroll;

public class ExaminationFeesOperations : ClinicOperations
{
    private readonly IMessageBus _bus;

    public ExaminationFeesOperations(IMessageBus bus) => _bus = bus;

    [ClinicPermission(PayrollReadCode)]
    [ClinicOperation("GET", "payroll/examination-fees/{id:guid}")]
    public async Task<ClinicResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ExaminationFeeDto>>(new GetExaminationFeeByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PayrollReadCode)]
    [ClinicOperation("POST", "payroll/examination-fees/all")]
    public async Task<ClinicResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<ExaminationFeeDto>>>(new GetExaminationFeesQuery(request), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PayrollFeesManageCode)]
    [ClinicOperation("POST", "payroll/examination-fees")]
    public async Task<ClinicResult> CreateAsync([FromBody] CreateExaminationFeeCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ExaminationFeeDto>>(command, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PayrollFeesManageCode)]
    [ClinicOperation("PUT", "payroll/examination-fees/{id:guid}")]
    public async Task<ClinicResult> UpdateAsync(Guid id, [FromBody] UpdateExaminationFeeCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { ExaminationFeeId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PayrollFeesManageCode)]
    [ClinicOperation("POST", "payroll/examination-fees/{id:guid}/activate")]
    public async Task<ClinicResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ActivateExaminationFeeCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PayrollFeesManageCode)]
    [ClinicOperation("POST", "payroll/examination-fees/{id:guid}/deactivate")]
    public async Task<ClinicResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivateExaminationFeeCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(PayrollFeesManageCode)]
    [ClinicOperation("DELETE", "payroll/examination-fees/{id:guid}")]
    public async Task<ClinicResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteExaminationFeeCommand(id), ct);
        return result.ToActionResult();
    }
}