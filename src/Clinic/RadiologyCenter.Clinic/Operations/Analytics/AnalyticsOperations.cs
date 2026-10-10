using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Examinations.Application.DTOs;
using RadiologyCenter.Examinations.Application.Queries.ExportAnalytics;
using RadiologyCenter.Examinations.Application.Queries.GetFinancialAnalytics;
using RadiologyCenter.Examinations.Application.Queries.GetFinancialExams;
using RadiologyCenter.Examinations.Application.Queries.GetMonthlyProfit;
using RadiologyCenter.Examinations.Application.Queries.GetOperationalAnalytics;
using RadiologyCenter.Examinations.Application.Queries.GetStaffMachineAnalytics;
using RadiologyCenter.Examinations.Application.Reports;

using RadiologyCenter.Clinic.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Clinic.Operations.Analytics;

public class AnalyticsOperations : ClinicOperations
{
    private readonly IMessageBus _bus;

    public AnalyticsOperations(IMessageBus bus) => _bus = bus;

    [ClinicPermission(AnalyticsReadCode)]
    [ClinicOperation("GET", "analytics/financial")]
    public async Task<ClinicResult> GetFinancialAsync(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<FinancialAnalyticsDto>>(new GetFinancialAnalyticsQuery(from, to), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(AnalyticsReadCode)]
    [ClinicOperation("GET", "analytics/financial/exams")]
    public async Task<ClinicResult> GetFinancialExamsAsync(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<IReadOnlyList<FinancialExamRowDto>>>(new GetFinancialExamsQuery(from, to), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(AnalyticsReadCode)]
    [ClinicOperation("GET", "analytics/operational")]
    public async Task<ClinicResult> GetOperationalAsync(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<OperationalAnalyticsDto>>(new GetOperationalAnalyticsQuery(from, to), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(AnalyticsReadCode)]
    [ClinicOperation("GET", "analytics/staff-machine")]
    public async Task<ClinicResult> GetStaffMachineAsync(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<StaffMachineAnalyticsDto>>(new GetStaffMachineAnalyticsQuery(from, to), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(AnalyticsReadCode)]
    [ClinicOperation("GET", "analytics/profit")]
    public async Task<ClinicResult> GetProfitAsync(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ProfitAnalyticsDto>>(new GetMonthlyProfitQuery(from, to), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(AnalyticsReadCode)]
    [ClinicOperation("GET", "analytics/insurance")]
    public async Task<ClinicResult> GetInsuranceAsync(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<InsuranceAnalyticsDto>>(new GetInsuranceAnalyticsQuery(from, to), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(AnalyticsReadCode)]
    [ClinicOperation("GET", "analytics/cash-flow")]
    public async Task<ClinicResult> GetCashFlowAsync(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<CashFlowReportDto>>(new GetCashFlowReportQuery(from, to), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(AnalyticsReadCode)]
    [ClinicOperation("GET", "analytics/export/financial")]
    public async Task<ClinicResult> ExportFinancialAsync(
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

    [ClinicPermission(AnalyticsReadCode)]
    [ClinicOperation("GET", "analytics/export/operational")]
    public async Task<ClinicResult> ExportOperationalAsync(
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

    [ClinicPermission(AnalyticsReadCode)]
    [ClinicOperation("GET", "analytics/export/staff")]
    public async Task<ClinicResult> ExportStaffAsync(
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

    [ClinicPermission(AnalyticsReadCode)]
    [ClinicOperation("GET", "analytics/export/profit")]
    public async Task<ClinicResult> ExportProfitAsync(
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

    [ClinicPermission(AnalyticsReadCode)]
    [ClinicOperation("GET", "analytics/export/insurance")]
    public async Task<ClinicResult> ExportInsuranceAsync(
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

    [ClinicPermission(AnalyticsReadCode)]
    [ClinicOperation("GET", "analytics/export/cash-flow")]
    public async Task<ClinicResult> ExportCashFlowAsync(
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
