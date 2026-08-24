using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Commands.AddPayslip;
using RadiologyCenter.Payroll.Application.Commands.ApprovePayRun;
using RadiologyCenter.Payroll.Application.Commands.ComputePayRun;
using RadiologyCenter.Payroll.Application.Commands.CreatePayRun;
using RadiologyCenter.Payroll.Application.Commands.DeletePayRun;
using RadiologyCenter.Payroll.Application.Commands.PayPayRun;
using RadiologyCenter.Payroll.Application.Commands.RejectPayRun;
using RadiologyCenter.Payroll.Application.Commands.RemovePayslip;
using RadiologyCenter.Payroll.Application.Commands.RestartPayRun;
using RadiologyCenter.Payroll.Application.DTOs;
using RadiologyCenter.Payroll.Application.Queries.GetPayRunById;
using RadiologyCenter.Payroll.Application.Queries.GetPayRuns;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Payroll;

[ApiController]
[Route("api/payroll/payruns")]
public class PayRunsController : ControllerBase
{
    private readonly IMessageBus _bus;

    public PayRunsController(IMessageBus bus) => _bus = bus;

    [HasPermission(PayrollReadCode)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PayRunDto>>(new GetPayRunByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollReadCode)]
    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<PayRunDto>>>(new GetPayRunsQuery(request), ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollPayRunsManageCode)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreatePayRunCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PayRunDto>>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollPayRunsManageCode)]
    [HttpPost("{id:guid}/payslips")]
    public async Task<IActionResult> AddPayslipAsync(Guid id, [FromBody] AddPayslipCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PayslipDto>>(command with { PayRunId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollPayRunsManageCode)]
    [HttpDelete("{id:guid}/payslips/{staffId:guid}")]
    public async Task<IActionResult> RemovePayslipAsync(Guid id, Guid staffId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new RemovePayslipCommand(id, staffId), ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollPayRunsRunCode)]
    [HttpPost("{id:guid}/compute")]
    public async Task<IActionResult> ComputeAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ComputePayRunCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollPayRunsRunCode)]
    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> ApproveAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ApprovePayRunCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollPayRunsRunCode)]
    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> RejectAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new RejectPayRunCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollPayRunsRunCode)]
    [HttpPost("{id:guid}/restart")]
    public async Task<IActionResult> RestartAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new RestartPayRunCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollPayRunsRunCode)]
    [HttpPost("{id:guid}/pay")]
    public async Task<IActionResult> PayAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new PayPayRunCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollPayRunsManageCode)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeletePayRunCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(PayrollReadCode)]
    [HttpGet("{id:guid}/payslips/{staffId:guid}/pdf")]
    public async Task<IActionResult> GetPayslipPdfAsync(Guid id, Guid staffId, [FromServices] IPayslipPdfService pdfService, CancellationToken ct)
    {
        var pdfBytes = await pdfService.GeneratePayslipPdfAsync(id, staffId, ct);
        return File(pdfBytes, "application/pdf", $"payslip-{id}-{staffId}.pdf");
    }
}