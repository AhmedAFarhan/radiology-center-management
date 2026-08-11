using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Examinations.Application.Commands.AddExaminationItem;
using RadiologyCenter.Examinations.Application.Commands.CancelExamination;
using RadiologyCenter.Examinations.Application.Commands.CheckInExamination;
using RadiologyCenter.Examinations.Application.Commands.CompleteExamination;
using RadiologyCenter.Examinations.Application.Commands.CreateExamination;
using RadiologyCenter.Examinations.Application.Commands.RecordExaminationPayment;
using RadiologyCenter.Examinations.Application.Commands.RemoveExaminationItem;
using RadiologyCenter.Examinations.Application.Commands.ScheduleExamination;
using RadiologyCenter.Examinations.Application.Commands.StartExamination;
using RadiologyCenter.Examinations.Application.Commands.UpdateExamination;
using RadiologyCenter.Examinations.Application.DTOs;
using RadiologyCenter.Examinations.Application.Queries.GetExaminationById;
using RadiologyCenter.Examinations.Application.Queries.GetExaminations;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Examinations;

[ApiController]
[Route("api/examinations")]
public class ExaminationsController : ControllerBase
{
    private readonly IMessageBus _bus;

    public ExaminationsController(IMessageBus bus) => _bus = bus;

    [HasPermission(ExaminationsReadCode)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ExaminationDto>>(new GetExaminationByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsReadCode)]
    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<ExaminationDto>>>(new GetExaminationsQuery(request), ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsCreateCode)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateExaminationCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ExaminationDto>>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsUpdateCode)]
    [HttpPut("{examinationId:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid examinationId, [FromBody] UpdateExaminationCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { ExaminationId = examinationId }, ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsUpdateCode)]
    [HttpPost("{examinationId:guid}/schedule")]
    public async Task<IActionResult> ScheduleAsync(Guid examinationId, [FromBody] ScheduleExaminationCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { ExaminationId = examinationId }, ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsPerformCode)]
    [HttpPost("{examinationId:guid}/check-in")]
    public async Task<IActionResult> CheckInAsync(Guid examinationId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new CheckInExaminationCommand(examinationId), ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsPerformCode)]
    [HttpPost("{examinationId:guid}/start")]
    public async Task<IActionResult> StartAsync(Guid examinationId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new StartExaminationCommand(examinationId), ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsPerformCode)]
    [HttpPost("{examinationId:guid}/complete")]
    public async Task<IActionResult> CompleteAsync(Guid examinationId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new CompleteExaminationCommand(examinationId), ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsCancelCode)]
    [HttpPost("{examinationId:guid}/cancel")]
    public async Task<IActionResult> CancelAsync(Guid examinationId, [FromBody] CancelExaminationCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { ExaminationId = examinationId }, ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsUpdateCode)]
    [HttpPost("{examinationId:guid}/items")]
    public async Task<IActionResult> AddItemAsync(Guid examinationId, [FromBody] AddExaminationItemCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ExaminationItemDto>>(command with { ExaminationId = examinationId }, ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsUpdateCode)]
    [HttpPost("{examinationId:guid}/payments")]
    public async Task<IActionResult> RecordPaymentAsync(Guid examinationId, [FromBody] RecordExaminationPaymentCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { ExaminationId = examinationId }, ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsUpdateCode)]
    [HttpDelete("{examinationId:guid}/items/{examinationItemId:guid}")]
    public async Task<IActionResult> RemoveItemAsync(Guid examinationId, Guid examinationItemId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new RemoveExaminationItemCommand(examinationId, examinationItemId), ct);
        return result.ToActionResult();
    }
}
