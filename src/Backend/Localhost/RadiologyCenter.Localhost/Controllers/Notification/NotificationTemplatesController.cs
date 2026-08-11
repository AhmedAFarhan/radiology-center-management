using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using RadiologyCenter.Notification.Application.Commands.NotificationTemplates.ActivateNotificationTemplate;
using RadiologyCenter.Notification.Application.Commands.NotificationTemplates.CreateNotificationTemplate;
using RadiologyCenter.Notification.Application.Commands.NotificationTemplates.DeactivateNotificationTemplate;
using RadiologyCenter.Notification.Application.Commands.NotificationTemplates.DeleteNotificationTemplate;
using RadiologyCenter.Notification.Application.Commands.NotificationTemplates.UpdateNotificationTemplate;
using RadiologyCenter.Notification.Application.DTOs;
using RadiologyCenter.Notification.Application.Queries.NotificationTemplates.GetNotificationTemplateById;
using RadiologyCenter.Notification.Application.Queries.NotificationTemplates.GetNotificationTemplates;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Notification;

[ApiController]
[Route("api/notifications/templates")]
public class NotificationTemplatesController : ControllerBase
{
    private readonly IMessageBus _bus;

    public NotificationTemplatesController(IMessageBus bus) => _bus = bus;

    [HasPermission(NotificationTemplatesManageCode)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateNotificationTemplateCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<NotificationTemplateDto>>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(NotificationTemplatesManageCode)]
    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<NotificationTemplateDto>>>(new GetNotificationTemplatesQuery(request), ct);
        return result.ToActionResult();
    }

    [HasPermission(NotificationTemplatesManageCode)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<NotificationTemplateDto>>(new GetNotificationTemplateByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(NotificationTemplatesManageCode)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateNotificationTemplateCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<NotificationTemplateDto>>(command with { Id = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(NotificationTemplatesManageCode)]
    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<NotificationTemplateDto>>(new ActivateNotificationTemplateCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(NotificationTemplatesManageCode)]
    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivateNotificationTemplateCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(NotificationTemplatesManageCode)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteNotificationTemplateCommand(id), ct);
        return result.ToActionResult();
    }
}