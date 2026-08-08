using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
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

namespace RadiologyCenter.Localhost.Controllers.Reports;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly IMessageBus _bus;

    public ReportsController(IMessageBus bus) => _bus = bus;

    [HasPermission(ReportsReadCode)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportDto>>(new GetReportByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(ReportsReadCode)]
    [HttpGet("by-examination/{examinationId:guid}")]
    public async Task<IActionResult> GetByExaminationAsync(Guid examinationId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportDto>>(new GetReportByExaminationQuery(examinationId), ct);
        return result.ToActionResult();
    }

    [HasPermission(ReportsReadCode)]
    [HttpGet("{id:guid}/versions")]
    public async Task<IActionResult> GetVersionsAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<IReadOnlyList<ReportVersionDto>>>(new GetReportVersionsQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(ReportsReadCode)]
    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<ReportListItemDto>>>(new GetReportsQuery(request), ct);
        return result.ToActionResult();
    }

    [HasPermission(ReportsCreateCode)]
    [HttpPost]
    public async Task<IActionResult> CreateDraftAsync([FromBody] CreateReportDraftCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportDto>>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(ReportsUpdateCode)]
    [HttpPut("{id:guid}/sections")]
    public async Task<IActionResult> UpsertSectionAsync(Guid id, [FromBody] UpsertReportSectionCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportDto>>(command with { ReportId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(ReportsUpdateCode)]
    [HttpPost("{id:guid}/findings")]
    public async Task<IActionResult> AddFindingAsync(Guid id, [FromBody] AddReportFindingCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportFindingDto>>(command with { ReportId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(ReportsUpdateCode)]
    [HttpPut("{id:guid}/findings/{findingId:guid}")]
    public async Task<IActionResult> UpdateFindingAsync(Guid id, Guid findingId, [FromBody] UpdateReportFindingCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { ReportId = id, FindingId = findingId }, ct);
        return result.ToActionResult();
    }

    [HasPermission(ReportsUpdateCode)]
    [HttpDelete("{id:guid}/findings/{findingId:guid}")]
    public async Task<IActionResult> RemoveFindingAsync(Guid id, Guid findingId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new RemoveReportFindingCommand(id, findingId), ct);
        return result.ToActionResult();
    }

    [HasPermission(ReportsUpdateCode)]
    [HttpPost("{id:guid}/apply-template")]
    public async Task<IActionResult> ApplyTemplateAsync(Guid id, [FromBody] ApplyReportTemplateCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportDto>>(command with { ReportId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(ReportsUpdateCode)]
    [HttpPost("{id:guid}/finalize")]
    public async Task<IActionResult> FinalizeAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportDto>>(new FinalizeReportCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(ReportsUpdateCode)]
    [HttpPost("{id:guid}/amend")]
    public async Task<IActionResult> AmendAsync(Guid id, [FromBody] AmendReportCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportDto>>(command with { ReportId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(ReportsUpdateCode)]
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> CancelAsync(Guid id, [FromBody] CancelReportCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { ReportId = id }, ct);
        return result.ToActionResult();
    }
}