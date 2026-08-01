using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Examinations.Application.Commands.ActivateExaminationType;
using RadiologyCenter.Examinations.Application.Commands.AddExaminationTypeItem;
using RadiologyCenter.Examinations.Application.Commands.CreateExaminationType;
using RadiologyCenter.Examinations.Application.Commands.DeactivateExaminationType;
using RadiologyCenter.Examinations.Application.Commands.DeleteExaminationType;
using RadiologyCenter.Examinations.Application.Commands.RemoveExaminationTypeItem;
using RadiologyCenter.Examinations.Application.Commands.UpdateExaminationType;
using RadiologyCenter.Examinations.Application.DTOs;
using RadiologyCenter.Examinations.Application.Queries.GetExaminationTypeById;
using RadiologyCenter.Examinations.Application.Queries.GetExaminationTypes;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Examinations;

[ApiController]
[Route("api/examinations/types")]
public class ExaminationTypesController : ControllerBase
{
    private readonly IMessageBus _bus;

    public ExaminationTypesController(IMessageBus bus) => _bus = bus;

    [HasPermission(ExaminationsReadCode)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ExaminationTypeDto>>(new GetExaminationTypeByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsReadCode)]
    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<ExaminationTypeDto>>>(new GetExaminationTypesQuery(request), ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsTypesManageCode)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateExaminationTypeCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ExaminationTypeDto>>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsTypesManageCode)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateExaminationTypeCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { ExaminationTypeId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsTypesManageCode)]
    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ActivateExaminationTypeCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsTypesManageCode)]
    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivateExaminationTypeCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsTypesManageCode)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteExaminationTypeCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsTypesManageCode)]
    [HttpPost("{id:guid}/items")]
    public async Task<IActionResult> AddItemAsync(Guid id, [FromBody] AddExaminationTypeItemCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ExaminationTypeItemDto>>(command with { ExaminationTypeId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(ExaminationsTypesManageCode)]
    [HttpDelete("{id:guid}/items/{examinationTypeItemId:guid}")]
    public async Task<IActionResult> RemoveItemAsync(Guid id, Guid examinationTypeItemId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new RemoveExaminationTypeItemCommand(id, examinationTypeItemId), ct);
        return result.ToActionResult();
    }
}
