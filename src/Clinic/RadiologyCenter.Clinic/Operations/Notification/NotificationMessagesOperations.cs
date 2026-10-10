using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;

using RadiologyCenter.Clinic.Extensions;
using RadiologyCenter.Notification.Application.Commands.Notifications.SendNotification;
using RadiologyCenter.Notification.Application.DTOs;
using RadiologyCenter.Notification.Application.Queries.NotificationMessages.GetNotificationMessages;
using RadiologyCenter.Notification.Application.Queries.NotificationMessages.PreviewNotification;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Clinic.Operations.Notification;

public class NotificationMessagesOperations : ClinicOperations
{
    private readonly IMessageBus _bus;

    public NotificationMessagesOperations(IMessageBus bus) => _bus = bus;

    [ClinicPermission(NotificationMessagesSendCode)]
    [ClinicOperation("POST", "notifications/send")]
    public async Task<ClinicResult> SendAsync([FromBody] SendNotificationCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(NotificationMessagesSendCode)]
    [ClinicOperation("POST", "notifications/preview")]
    public async Task<ClinicResult> PreviewAsync([FromBody] SendNotificationCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<NotificationMessageDto>>(new PreviewNotificationCommand(command), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(NotificationMessagesReadCode)]
    [ClinicOperation("POST", "notifications/messages/all")]
    public async Task<ClinicResult> GetAllAsync([FromBody] QueryRequest request, [FromQuery] string? channel, [FromQuery] string? status, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<NotificationMessageDto>>>(new GetNotificationMessagesQuery(request, channel, status), ct);
        return result.ToActionResult();
    }
}