using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Features.Notifications.Services;

public sealed class NotificationService : CrudServiceBase
{
    private const string TemplatesRes = "api/notifications/templates";

    public NotificationService(ApiClient api) : base(api) { }

    public Task<NotificationTemplateDto> CreateTemplateAsync(NotificationTemplateInput input, CancellationToken ct = default)
        => CreateEntityAsync<NotificationTemplateDto>(TemplatesRes, input, ct);

    public Task<NotificationTemplateDto> UpdateTemplateAsync(string id, NotificationTemplateInput input, CancellationToken ct = default)
        => Api.PutAsync<NotificationTemplateDto>($"{TemplatesRes}/{id}", input, ct);

    public Task<NotificationTemplateDto> GetTemplateByIdAsync(string id, CancellationToken ct = default)
        => FetchByIdAsync<NotificationTemplateDto>(TemplatesRes, id, ct);

    public Task<PagedResult<NotificationTemplateDto>> GetTemplatesPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => FetchPageAsync<NotificationTemplateDto>(TemplatesRes, searchTerm, sortBy, sortDescending, pageNumber, pageSize, ct);

    public Task<NotificationTemplateDto> ActivateTemplateAsync(string id, CancellationToken ct = default)
        => Api.PostAsync<NotificationTemplateDto>($"{TemplatesRes}/{id}/activate", null, ct);

    public Task DeactivateTemplateAsync(string id, CancellationToken ct = default)
        => Api.SendAsync($"{TemplatesRes}/{id}/deactivate", null, ct);

    public Task DeleteTemplateAsync(string id, CancellationToken ct = default)
        => DeleteEntityAsync(TemplatesRes, id, ct);

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

        return Api.PostAsync<PagedResult<NotificationMessageDto>>(url, PagedQuery(searchTerm, sortBy, sortDescending, pageNumber, pageSize), ct);
    }

    public Task SendAsync(SendNotificationInput input, CancellationToken ct = default)
        => Api.SendAsync("api/notifications/send", input, ct);

    public Task<NotificationPreviewDto> PreviewAsync(SendNotificationInput input, CancellationToken ct = default)
        => Api.PostAsync<NotificationPreviewDto>("api/notifications/preview", input, ct);
}
