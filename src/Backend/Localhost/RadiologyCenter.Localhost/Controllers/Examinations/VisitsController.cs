using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Examinations.Application.Commands.AddExaminationItem;
using RadiologyCenter.Examinations.Application.Commands.AddExaminationToVisit;
using RadiologyCenter.Examinations.Application.Commands.CancelExamination;
using RadiologyCenter.Examinations.Application.Commands.CancelVisit;
using RadiologyCenter.Examinations.Application.Commands.CheckInExamination;
using RadiologyCenter.Examinations.Application.Commands.CompleteExamination;
using RadiologyCenter.Examinations.Application.Commands.CreateVisit;
using RadiologyCenter.Examinations.Application.Commands.RemoveExaminationItem;
using RadiologyCenter.Examinations.Application.Commands.ScheduleExamination;
using RadiologyCenter.Examinations.Application.Commands.StartExamination;
using RadiologyCenter.Examinations.Application.Commands.UpdateExamination;
using RadiologyCenter.Examinations.Application.Commands.UpdateExaminationItem;
using RadiologyCenter.Examinations.Application.DTOs;
using RadiologyCenter.Examinations.Application.Queries.GetVisitById;
using RadiologyCenter.Examinations.Application.Queries.GetVisits;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Examinations;

[ApiController]
[Route("api/examinations/visits")]
public class VisitsController : ControllerBase
{
    private readonly IMessageBus _bus;

    public VisitsController(IMessageBus bus) => _bus = bus;

    [HasPermission(ExaminationsReadCode)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<VisitDto>>(new GetVisitByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsReadCode)]
    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<VisitDto>>>(new GetVisitsQuery(request), ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsCreateCode)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateVisitCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<VisitDto>>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsCreateCode)]
    [HttpPost("{visitId:guid}/examinations")]
    public async Task<IActionResult> AddExaminationAsync(Guid visitId, [FromBody] AddExaminationToVisitCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ExaminationDto>>(command with { VisitId = visitId }, ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsUpdateCode)]
    [HttpPut("{visitId:guid}/examinations/{examinationId:guid}")]
    public async Task<IActionResult> UpdateExaminationAsync(Guid visitId, Guid examinationId, [FromBody] UpdateExaminationCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { VisitId = visitId, ExaminationId = examinationId }, ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsUpdateCode)]
    [HttpPost("{visitId:guid}/examinations/{examinationId:guid}/schedule")]
    public async Task<IActionResult> ScheduleAsync(Guid visitId, Guid examinationId, [FromBody] ScheduleExaminationCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { VisitId = visitId, ExaminationId = examinationId }, ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsPerformCode)]
    [HttpPost("{visitId:guid}/examinations/{examinationId:guid}/check-in")]
    public async Task<IActionResult> CheckInAsync(Guid visitId, Guid examinationId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new CheckInExaminationCommand(visitId, examinationId), ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsPerformCode)]
    [HttpPost("{visitId:guid}/examinations/{examinationId:guid}/start")]
    public async Task<IActionResult> StartAsync(Guid visitId, Guid examinationId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new StartExaminationCommand(visitId, examinationId), ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsPerformCode)]
    [HttpPost("{visitId:guid}/examinations/{examinationId:guid}/complete")]
    public async Task<IActionResult> CompleteAsync(Guid visitId, Guid examinationId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new CompleteExaminationCommand(visitId, examinationId), ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsCancelCode)]
    [HttpPost("{visitId:guid}/examinations/{examinationId:guid}/cancel")]
    public async Task<IActionResult> CancelAsync(Guid visitId, Guid examinationId, [FromBody] CancelExaminationCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { VisitId = visitId, ExaminationId = examinationId }, ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsCancelCode)]
    [HttpPost("{visitId:guid}/cancel")]
    public async Task<IActionResult> CancelVisitAsync(Guid visitId, [FromBody] CancelVisitCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { VisitId = visitId }, ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsUpdateCode)]
    [HttpPost("{visitId:guid}/examinations/{examinationId:guid}/items")]
    public async Task<IActionResult> AddItemAsync(Guid visitId, Guid examinationId, [FromBody] AddExaminationItemCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ExaminationItemDto>>(command with { VisitId = visitId, ExaminationId = examinationId }, ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsUpdateCode)]
    [HttpPut("{visitId:guid}/examinations/{examinationId:guid}/items/{examinationItemId:guid}")]
    public async Task<IActionResult> UpdateItemAsync(Guid visitId, Guid examinationId, Guid examinationItemId, [FromBody] UpdateExaminationItemCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { VisitId = visitId, ExaminationId = examinationId, ExaminationItemId = examinationItemId }, ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsUpdateCode)]
    [HttpDelete("{visitId:guid}/examinations/{examinationId:guid}/items/{examinationItemId:guid}")]
    public async Task<IActionResult> RemoveItemAsync(Guid visitId, Guid examinationId, Guid examinationItemId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new RemoveExaminationItemCommand(visitId, examinationId, examinationItemId), ct);
        return result.ToActionResult();
    }
}
