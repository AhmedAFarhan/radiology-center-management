using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Services;

public sealed class NotificationService
{
    private readonly ApiClient _api;

    public NotificationService(ApiClient api) => _api = api;

    public Task<NotificationTemplateDto> CreateTemplateAsync(NotificationTemplateInput input, CancellationToken ct = default)
        => _api.PostAsync<NotificationTemplateDto>("api/notifications/templates", input, ct);

    public Task<NotificationTemplateDto> UpdateTemplateAsync(string id, NotificationTemplateInput input, CancellationToken ct = default)
        => _api.PutAsync<NotificationTemplateDto>($"api/notifications/templates/{id}", input, ct);

    public Task<NotificationTemplateDto> GetTemplateByIdAsync(string id, CancellationToken ct = default)
        => _api.GetAsync<NotificationTemplateDto>($"api/notifications/templates/{id}", ct);

    public Task<PagedResult<NotificationTemplateDto>> GetTemplatesPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = new
        {
            pagination = new { pageNumber, pageSize },
            sortBy,
            sortDescending,
            searchTerm,
        };

        return _api.PostAsync<PagedResult<NotificationTemplateDto>>("api/notifications/templates/all", query, ct);
    }

    public Task<NotificationTemplateDto> ActivateTemplateAsync(string id, CancellationToken ct = default)
        => _api.PostAsync<NotificationTemplateDto>($"api/notifications/templates/{id}/activate", null, ct);

    public Task DeactivateTemplateAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/notifications/templates/{id}/deactivate", null, ct);

    public Task DeleteTemplateAsync(string id, CancellationToken ct = default)
        => _api.SendDeleteAsync($"api/notifications/templates/{id}", ct);

    public Task<PagedResult<NotificationMessageDto>> GetMessagesPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        string? channel,
        string? status,
        CancellationToken ct = default)
    {
        var query = new
        {
            pagination = new { pageNumber, pageSize },
            sortBy,
            sortDescending,
            searchTerm,
        };

        var url = "api/notifications/messages/all";
        var separator = "?";

        if (!string.IsNullOrWhiteSpace(channel))
        {
            url += $"{separator}channel={Uri.EscapeDataString(channel)}";
            separator = "&";
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            url += $"{separator}status={Uri.EscapeDataString(status)}";
        }

        return _api.PostAsync<PagedResult<NotificationMessageDto>>(url, query, ct);
    }

    public Task SendAsync(SendNotificationInput input, CancellationToken ct = default)
        => _api.SendAsync("api/notifications/send", input, ct);

    public Task<NotificationPreviewDto> PreviewAsync(SendNotificationInput input, CancellationToken ct = default)
        => _api.PostAsync<NotificationPreviewDto>("api/notifications/preview", input, ct);
}