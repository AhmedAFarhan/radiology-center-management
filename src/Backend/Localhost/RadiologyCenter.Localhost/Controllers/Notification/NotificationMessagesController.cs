using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using RadiologyCenter.Notification.Application.Commands.Notifications.SendNotification;
using RadiologyCenter.Notification.Application.DTOs;
using RadiologyCenter.Notification.Application.Queries.NotificationMessages.GetNotificationMessages;
using RadiologyCenter.Notification.Application.Queries.NotificationMessages.PreviewNotification;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Notification;

[ApiController]
[Route("api/notifications")]
public class NotificationMessagesController : ControllerBase
{
    private readonly IMessageBus _bus;

    public NotificationMessagesController(IMessageBus bus) => _bus = bus;

    [HasPermission(NotificationMessagesSendCode)]
    [HttpPost("send")]
    public async Task<IActionResult> SendAsync([FromBody] SendNotificationCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(NotificationMessagesSendCode)]
    [HttpPost("preview")]
    public async Task<IActionResult> PreviewAsync([FromBody] SendNotificationCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<NotificationMessageDto>>(new PreviewNotificationCommand(command), ct);
        return result.ToActionResult();
    }

    [HasPermission(NotificationMessagesReadCode)]
    [HttpPost("messages/all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, [FromQuery] string? channel, [FromQuery] string? status, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<NotificationMessageDto>>>(new GetNotificationMessagesQuery(request, channel, status), ct);
        return result.ToActionResult();
    }
}