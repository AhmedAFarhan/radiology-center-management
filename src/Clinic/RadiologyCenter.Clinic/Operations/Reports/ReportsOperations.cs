using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;

using RadiologyCenter.Clinic.Extensions;
using RadiologyCenter.Reports.Application.Commands.AddReportFinding;
using RadiologyCenter.Reports.Application.Commands.AmendReport;
using RadiologyCenter.Reports.Application.Commands.ApplyReportTemplate;
using RadiologyCenter.Reports.Application.Commands.CancelReport;
using RadiologyCenter.Reports.Application.Commands.CreateReportDraft;
using RadiologyCenter.Reports.Application.Commands.FinalizeReport;
using RadiologyCenter.Reports.Application.Commands.RemoveReportFinding;
using RadiologyCenter.Reports.Application.Commands.UpdateReportFinding;
using RadiologyCenter.Reports.Application.Commands.UpsertReportSection;
using RadiologyCenter.Reports.Application.DTOs;
using RadiologyCenter.Reports.Application.Queries.GetReportById;
using RadiologyCenter.Reports.Application.Queries.GetReportByExamination;
using RadiologyCenter.Reports.Application.Queries.GetReportVersions;
using RadiologyCenter.Reports.Application.Queries.GetReports;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Clinic.Operations.Reports;

public class ReportsOperations : ClinicOperations
{
    private readonly IMessageBus _bus;

    public ReportsOperations(IMessageBus bus) => _bus = bus;

    [ClinicPermission(ReportsReadCode)]
    [ClinicOperation("GET", "reports/{id:guid}")]
    public async Task<ClinicResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportDto>>(new GetReportByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ReportsReadCode)]
    [ClinicOperation("GET", "reports/by-examination/{examinationId:guid}")]
    public async Task<ClinicResult> GetByExaminationAsync(Guid examinationId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportDto>>(new GetReportByExaminationQuery(examinationId), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ReportsReadCode)]
    [ClinicOperation("GET", "reports/{id:guid}/versions")]
    public async Task<ClinicResult> GetVersionsAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<IReadOnlyList<ReportVersionDto>>>(new GetReportVersionsQuery(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ReportsReadCode)]
    [ClinicOperation("POST", "reports/all")]
    public async Task<ClinicResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<ReportListItemDto>>>(new GetReportsQuery(request), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ReportsCreateCode)]
    [ClinicOperation("POST", "reports")]
    public async Task<ClinicResult> CreateDraftAsync([FromBody] CreateReportDraftCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportDto>>(command, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ReportsUpdateCode)]
    [ClinicOperation("PUT", "reports/{id:guid}/sections")]
    public async Task<ClinicResult> UpsertSectionAsync(Guid id, [FromBody] UpsertReportSectionCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportDto>>(command with { ReportId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ReportsUpdateCode)]
    [ClinicOperation("POST", "reports/{id:guid}/findings")]
    public async Task<ClinicResult> AddFindingAsync(Guid id, [FromBody] AddReportFindingCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportFindingDto>>(command with { ReportId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ReportsUpdateCode)]
    [ClinicOperation("PUT", "reports/{id:guid}/findings/{findingId:guid}")]
    public async Task<ClinicResult> UpdateFindingAsync(Guid id, Guid findingId, [FromBody] UpdateReportFindingCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { ReportId = id, FindingId = findingId }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ReportsUpdateCode)]
    [ClinicOperation("DELETE", "reports/{id:guid}/findings/{findingId:guid}")]
    public async Task<ClinicResult> RemoveFindingAsync(Guid id, Guid findingId, [FromQuery] int? expectedRevision, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new RemoveReportFindingCommand(id, findingId) { ExpectedRevision = expectedRevision }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ReportsUpdateCode)]
    [ClinicOperation("POST", "reports/{id:guid}/apply-template")]
    public async Task<ClinicResult> ApplyTemplateAsync(Guid id, [FromBody] ApplyReportTemplateCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportDto>>(command with { ReportId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ReportsUpdateCode)]
    [ClinicOperation("POST", "reports/{id:guid}/finalize")]
    public async Task<ClinicResult> FinalizeAsync(Guid id, [FromBody] FinalizeReportCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportDto>>(command with { ReportId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ReportsUpdateCode)]
    [ClinicOperation("POST", "reports/{id:guid}/amend")]
    public async Task<ClinicResult> AmendAsync(Guid id, [FromBody] AmendReportCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportDto>>(command with { ReportId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ReportsUpdateCode)]
    [ClinicOperation("POST", "reports/{id:guid}/cancel")]
    public async Task<ClinicResult> CancelAsync(Guid id, [FromBody] CancelReportCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { ReportId = id }, ct);
        return result.ToActionResult();
    }
}