namespace RadiologyCenter.BuildingBlocks.Application.Abstractions.Services;

public interface INotificationService
{
    Task BroadcastAsync(string topic, object payload, CancellationToken ct = default);
    Task SendToGroupAsync(string group, string topic, object payload, CancellationToken ct = default);
}
