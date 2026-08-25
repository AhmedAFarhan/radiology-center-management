using Microsoft.AspNetCore.SignalR;
using RadiologyCenter.BuildingBlocks.Application.Abstractions.Services;

namespace RadiologyCenter.BuildingBlocks.Infrastructure.RealTime;

public sealed class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hub;

    public NotificationService(IHubContext<NotificationHub> hub)
    {
        _hub = hub;
    }

    public async Task BroadcastAsync(string topic, object payload, CancellationToken ct = default)
    {
        await _hub.Clients.All.SendAsync(topic, payload, ct);
    }

    public async Task SendToGroupAsync(string group, string topic, object payload, CancellationToken ct = default)
    {
        await _hub.Clients.Group(group).SendAsync(topic, payload, ct);
    }
}
