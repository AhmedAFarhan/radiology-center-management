using System.Net.Http;
using System.Net.Http.Json;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using MudBlazor;
using RadiologyCenter.Desktop;
using RadiologyCenter.Desktop.Components;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Components.Pages.Notification;

public partial class NotificationTemplates : ListPageBase<NotificationTemplateDto>
{
    protected override string UnreachableMessage => T.Notifications.Unreachable;

    protected override async Task<PagedResult<NotificationTemplateDto>> LoadPageAsync(
        string? search,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken ct)
        => await NotificationService.GetTemplatesPagedAsync(search, sortBy, sortDescending, page, pageSize, ct);

    private async Task OpenCreateDialogAsync()
    {
        var dialog = await DialogService.ShowAsync<NotificationTemplateEditorDialog>(T.Notifications.NewTemplateDialogTitle, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(NotificationTemplateDto template)
    {
        var parameters = new DialogParameters { ["Template"] = template };
        var dialog = await DialogService.ShowAsync<NotificationTemplateEditorDialog>(T.FormatValue(T.Notifications.EditTemplateTitle, template.Name), parameters, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task ToggleActiveAsync(NotificationTemplateDto template)
    {
        await SafeExecute.RunAsync(async () =>
            {
                if (template.IsActive)
                    await NotificationService.DeactivateTemplateAsync(template.Id);
                else
                    await NotificationService.ActivateTemplateAsync(template.Id);

                Snackbar.Add(template.IsActive ? T.Notifications.Deactivated : T.Notifications.Activated, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Notifications.Unreachable);
    }

    private async Task DeleteTemplateAsync(NotificationTemplateDto template)
    {
        var confirmed = await DialogService.ShowMessageBoxAsync(
            T.Notifications.DeleteTitle,
            T.FormatValue(T.Notifications.DeleteConfirm, template.Name),
            T.Common.Delete,
            T.Common.Cancel);

        if (confirmed != true)
            return;

        await SafeExecute.RunAsync(async () =>
            {
                await NotificationService.DeleteTemplateAsync(template.Id);
                Snackbar.Add(T.Notifications.Deleted, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Notifications.Unreachable);
    }
}