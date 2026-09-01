using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Examinations.Application.DTOs;
using RadiologyCenter.Examinations.Application.Queries.ExportAnalytics;
using RadiologyCenter.Examinations.Application.Queries.GetFinancialAnalytics;
using RadiologyCenter.Examinations.Application.Queries.GetFinancialExams;
using RadiologyCenter.Examinations.Application.Queries.GetMonthlyProfit;
using RadiologyCenter.Examinations.Application.Queries.GetOperationalAnalytics;
using RadiologyCenter.Examinations.Application.Queries.GetStaffMachineAnalytics;
using RadiologyCenter.Examinations.Application.Reports;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Analytics;

[ApiController]
[Route("api/analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly IMessageBus _bus;

    public AnalyticsController(IMessageBus bus) => _bus = bus;

    [HasPermission(AnalyticsReadCode)]
    [HttpGet("financial")]
    public async Task<IActionResult> GetFinancialAsync(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<FinancialAnalyticsDto>>(new GetFinancialAnalyticsQuery(from, to), ct);
        return result.ToActionResult();
    }

    [HasPermission(AnalyticsReadCode)]
    [HttpGet("financial/exams")]
    public async Task<IActionResult> GetFinancialExamsAsync(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<IReadOnlyList<FinancialExamRowDto>>>(new GetFinancialExamsQuery(from, to), ct);
        return result.ToActionResult();
    }

    [HasPermission(AnalyticsReadCode)]
    [HttpGet("operational")]
    public async Task<IActionResult> GetOperationalAsync(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<OperationalAnalyticsDto>>(new GetOperationalAnalyticsQuery(from, to), ct);
        return result.ToActionResult();
    }

    [HasPermission(AnalyticsReadCode)]
    [HttpGet("staff-machine")]
    public async Task<IActionResult> GetStaffMachineAsync(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<StaffMachineAnalyticsDto>>(new GetStaffMachineAnalyticsQuery(from, to), ct);
        return result.ToActionResult();
    }

    [HasPermission(AnalyticsReadCode)]
    [HttpGet("profit")]
    public async Task<IActionResult> GetProfitAsync(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ProfitAnalyticsDto>>(new GetMonthlyProfitQuery(from, to), ct);
        return result.ToActionResult();
    }

    [HasPermission(AnalyticsReadCode)]
    [HttpGet("insurance")]
    public async Task<IActionResult> GetInsuranceAsync(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<InsuranceAnalyticsDto>>(new GetInsuranceAnalyticsQuery(from, to), ct);
        return result.ToActionResult();
    }

    [HasPermission(AnalyticsReadCode)]
    [HttpGet("cash-flow")]
    public async Task<IActionResult> GetCashFlowAsync(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<CashFlowReportDto>>(new GetCashFlowReportQuery(from, to), ct);
        return result.ToActionResult();
    }

    [HasPermission(AnalyticsReadCode)]
    [HttpGet("export/financial")]
    public async Task<IActionResult> ExportFinancialAsync(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] ReportFormat format,
        CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportContentDto>>(new ExportFinancialReportQuery(from, to, format), ct);
        return result.IsSuccess
            ? File(result.Value.Content, result.Value.ContentType, result.Value.FileName)
            : result.ToActionResult();
    }

    [HasPermission(AnalyticsReadCode)]
    [HttpGet("export/operational")]
    public async Task<IActionResult> ExportOperationalAsync(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] ReportFormat format,
        CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportContentDto>>(new ExportOperationalReportQuery(from, to, format), ct);
        return result.IsSuccess
            ? File(result.Value.Content, result.Value.ContentType, result.Value.FileName)
            : result.ToActionResult();
    }

    [HasPermission(AnalyticsReadCode)]
    [HttpGet("export/staff")]
    public async Task<IActionResult> ExportStaffAsync(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] ReportFormat format,
        CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportContentDto>>(new ExportStaffReportQuery(from, to, format), ct);
        return result.IsSuccess
            ? File(result.Value.Content, result.Value.ContentType, result.Value.FileName)
            : result.ToActionResult();
    }

    [HasPermission(AnalyticsReadCode)]
    [HttpGet("export/profit")]
    public async Task<IActionResult> ExportProfitAsync(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] ReportFormat format,
        CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportContentDto>>(new ExportProfitReportQuery(from, to, format), ct);
        return result.IsSuccess
            ? File(result.Value.Content, result.Value.ContentType, result.Value.FileName)
            : result.ToActionResult();
    }

    [HasPermission(AnalyticsReadCode)]
    [HttpGet("export/insurance")]
    public async Task<IActionResult> ExportInsuranceAsync(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] ReportFormat format,
        CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportContentDto>>(new ExportInsuranceReportQuery(from, to, format), ct);
        return result.IsSuccess
            ? File(result.Value.Content, result.Value.ContentType, result.Value.FileName)
            : result.ToActionResult();
    }

    [HasPermission(AnalyticsReadCode)]
    [HttpGet("export/cash-flow")]
    public async Task<IActionResult> ExportCashFlowAsync(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] ReportFormat format,
        CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportContentDto>>(new ExportCashFlowReportQuery(from, to, format), ct);
        return result.IsSuccess
            ? File(result.Value.Content, result.Value.ContentType, result.Value.FileName)
            : result.ToActionResult();
    }
}
