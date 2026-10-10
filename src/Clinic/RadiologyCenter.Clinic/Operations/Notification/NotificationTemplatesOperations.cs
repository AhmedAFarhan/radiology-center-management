using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;

using RadiologyCenter.Clinic.Extensions;
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

namespace RadiologyCenter.Clinic.Operations.Notification;

public class NotificationTemplatesOperations : ClinicOperations
{
    private readonly IMessageBus _bus;

    public NotificationTemplatesOperations(IMessageBus bus) => _bus = bus;

    [ClinicPermission(NotificationTemplatesManageCode)]
    [ClinicOperation("POST", "notifications/templates")]
    public async Task<ClinicResult> CreateAsync([FromBody] CreateNotificationTemplateCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<NotificationTemplateDto>>(command, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(NotificationTemplatesManageCode)]
    [ClinicOperation("POST", "notifications/templates/all")]
    public async Task<ClinicResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<NotificationTemplateDto>>>(new GetNotificationTemplatesQuery(request), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(NotificationTemplatesManageCode)]
    [ClinicOperation("GET", "notifications/templates/{id:guid}")]
    public async Task<ClinicResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<NotificationTemplateDto>>(new GetNotificationTemplateByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(NotificationTemplatesManageCode)]
    [ClinicOperation("PUT", "notifications/templates/{id:guid}")]
    public async Task<ClinicResult> UpdateAsync(Guid id, [FromBody] UpdateNotificationTemplateCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<NotificationTemplateDto>>(command with { Id = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(NotificationTemplatesManageCode)]
    [ClinicOperation("POST", "notifications/templates/{id:guid}/activate")]
    public async Task<ClinicResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<NotificationTemplateDto>>(new ActivateNotificationTemplateCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(NotificationTemplatesManageCode)]
    [ClinicOperation("POST", "notifications/templates/{id:guid}/deactivate")]
    public async Task<ClinicResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivateNotificationTemplateCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(NotificationTemplatesManageCode)]
    [ClinicOperation("DELETE", "notifications/templates/{id:guid}")]
    public async Task<ClinicResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteNotificationTemplateCommand(id), ct);
        return result.ToActionResult();
    }
}