using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;

using RadiologyCenter.Clinic.Extensions;
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

namespace RadiologyCenter.Clinic.Operations.Reports;

public class ReportTemplatesOperations : ClinicOperations
{
    private readonly IMessageBus _bus;

    public ReportTemplatesOperations(IMessageBus bus) => _bus = bus;

    [ClinicPermission(ReportsReadCode)]
    [ClinicOperation("GET", "reports/templates/{id:guid}")]
    public async Task<ClinicResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportTemplateDto>>(new GetReportTemplateByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ReportsReadCode)]
    [ClinicOperation("POST", "reports/templates/all")]
    public async Task<ClinicResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<ReportTemplateDto>>>(new GetReportTemplatesQuery(request), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ReportsUpdateCode)]
    [ClinicOperation("POST", "reports/templates")]
    public async Task<ClinicResult> CreateAsync([FromBody] CreateReportTemplateCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportTemplateDto>>(command, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ReportsUpdateCode)]
    [ClinicOperation("PUT", "reports/templates/{id:guid}")]
    public async Task<ClinicResult> UpdateAsync(Guid id, [FromBody] UpdateReportTemplateCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportTemplateDto>>(command with { TemplateId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ReportsUpdateCode)]
    [ClinicOperation("POST", "reports/templates/{id:guid}/sections")]
    public async Task<ClinicResult> AddSectionAsync(Guid id, [FromBody] AddTemplateSectionCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportTemplateDto>>(command with { TemplateId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ReportsUpdateCode)]
    [ClinicOperation("DELETE", "reports/templates/{id:guid}/sections/{sectionId:guid}")]
    public async Task<ClinicResult> RemoveSectionAsync(Guid id, Guid sectionId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReportTemplateDto>>(new RemoveTemplateSectionCommand(id, sectionId), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ReportsUpdateCode)]
    [ClinicOperation("POST", "reports/templates/{id:guid}/activate")]
    public async Task<ClinicResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ActivateReportTemplateCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ReportsUpdateCode)]
    [ClinicOperation("POST", "reports/templates/{id:guid}/deactivate")]
    public async Task<ClinicResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivateReportTemplateCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ReportsDeleteCode)]
    [ClinicOperation("DELETE", "reports/templates/{id:guid}")]
    public async Task<ClinicResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteReportTemplateCommand(id), ct);
        return result.ToActionResult();
    }
}