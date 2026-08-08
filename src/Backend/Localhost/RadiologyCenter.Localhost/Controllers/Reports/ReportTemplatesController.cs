using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using RadiologyCenter.Reports.Application.Commands.ReportTemplates.ActivateReportTemplate;
using RadiologyCenter.Reports.Application.Commands.ReportTemplates.AddTemplateSection;
using RadiologyCenter.Reports.Application.Commands.ReportTemplates.CreateReportTemplate;
using RadiologyCenter.Reports.Application.Commands.ReportTemplates.DeactivateReportTemplate;
using RadiologyCenter.Reports.Application.Commands.ReportTemplates.DeleteReportTemplate;
using RadiologyCenter.Reports.Application.Commands.ReportTemplates.RemoveTemplateSection;
using RadiologyCenter.Reports.Application.Commands.ReportTemplates.UpdateReportTemplate;
using RadiologyCenter.Reports.Application.DTOs;
using RadiologyCenter.Reports.Application.Queries.GetReportTemplateById;
using RadiologyCenter.Reports.Application.Queries.GetReportTemplates;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Reports;

[ApiController]
[Route("api/reports/templates")]
public class ReportTemplatesController : ControllerBase
{
    private readonly IMessageBus _bus;

    public ReportTemplatesController(IMessageBus bus) => _bus = bus;

    [HasPermission(ReportsReadCode)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportTemplateDto>>(new GetReportTemplateByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(ReportsReadCode)]
    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<ReportTemplateDto>>>(new GetReportTemplatesQuery(request), ct);
        return result.ToActionResult();
    }

    [HasPermission(ReportsUpdateCode)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateReportTemplateCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportTemplateDto>>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(ReportsUpdateCode)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateReportTemplateCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportTemplateDto>>(command with { TemplateId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(ReportsUpdateCode)]
    [HttpPost("{id:guid}/sections")]
    public async Task<IActionResult> AddSectionAsync(Guid id, [FromBody] AddTemplateSectionCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportTemplateDto>>(command with { TemplateId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(ReportsUpdateCode)]
    [HttpDelete("{id:guid}/sections/{sectionId:guid}")]
    public async Task<IActionResult> RemoveSectionAsync(Guid id, Guid sectionId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportTemplateDto>>(new RemoveTemplateSectionCommand(id, sectionId), ct);
        return result.ToActionResult();
    }

    [HasPermission(ReportsUpdateCode)]
    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ActivateReportTemplateCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(ReportsUpdateCode)]
    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivateReportTemplateCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(ReportsDeleteCode)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteReportTemplateCommand(id), ct);
        return result.ToActionResult();
    }
}