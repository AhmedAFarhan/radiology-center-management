using System.Text.Json;
using Microsoft.AspNetCore.SignalR.Client;

namespace RadiologyCenter.Desktop.Services;

public sealed class RealTimeNotificationService : IAsyncDisposable
{
    private HubConnection? _connection;
    private readonly Dictionary<string, List<Func<JsonElement, Task>>> _handlers = new(StringComparer.OrdinalIgnoreCase);

    public bool IsConnected => _connection?.State == HubConnectionState.Connected;

    public async Task StartAsync(string hubUrl)
    {
        if (_connection is not null)
            return;

        _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        _connection.Reconnecting += _ => Task.CompletedTask;

        _connection.Reconnected += async _ =>
        {
            foreach (var topic in _handlers.Keys)
                await _connection.SendAsync("JoinGroup", topic);
        };

        await _connection.StartAsync();

        foreach (var topic in _handlers.Keys)
            await _connection.SendAsync("JoinGroup", topic);
    }

    public void On<T>(string topic, Func<T, Task> handler)
    {
        if (!_handlers.ContainsKey(topic))
            _handlers[topic] = new List<Func<JsonElement, Task>>();

        _handlers[topic].Add(async json =>
        {
            var obj = JsonSerializer.Deserialize<T>(json.GetRawText());
            if (obj is not null)
                await handler(obj);
        });

        if (_connection is not null)
        {
            _connection.On<JsonElement>(topic, async json =>
            {
                foreach (var h in _handlers[topic])
                    await h(json);
            });

            _ = _connection.SendAsync("JoinGroup", topic);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.DisposeAsync();
            _connection = null;
        }
    }
}
