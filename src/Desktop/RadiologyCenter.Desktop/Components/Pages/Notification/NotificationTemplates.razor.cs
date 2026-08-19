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

public partial class NotificationTemplates : ComponentBase, IDisposable
{
private MudTable<NotificationTemplateDto>? _table;
    private string? _search;
    private CancellationTokenSource? _searchCts;
    private string? _loadError;
    private bool _offline;

    private async Task<TableData<NotificationTemplateDto>> LoadServerData(TableState state, CancellationToken ct)
    {
        try
        {
            var page = await NotificationService.GetTemplatesPagedAsync(
                _search,
                state.SortLabel,
                state.SortDirection == SortDirection.Descending,
                state.Page + 1,
                state.PageSize,
                ct);

            _loadError = null;
            _offline = false;
            return new TableData<NotificationTemplateDto> { Items = page.Items, TotalItems = page.TotalCount };
        }
        catch (OperationCanceledException)
        {
            if (ct.IsCancellationRequested)
                return new TableData<NotificationTemplateDto> { Items = Array.Empty<NotificationTemplateDto>(), TotalItems = 0 };
            throw;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            _loadError = ex.Message;
            _offline = false;
            return new TableData<NotificationTemplateDto> { Items = Array.Empty<NotificationTemplateDto>(), TotalItems = 0 };
        }
        catch (Exception)
        {
            Snackbar.Add(T.Notifications.Unreachable, Severity.Error);
            _loadError = T.Notifications.Unreachable;
            _offline = true;
            return new TableData<NotificationTemplateDto> { Items = Array.Empty<NotificationTemplateDto>(), TotalItems = 0 };
        }
    }

    private async Task OnSearchChanged(string? value)
    {
        _search = value;

        _searchCts?.Cancel();
        var cts = _searchCts = new CancellationTokenSource();
        try
        {
            await Task.Delay(400, cts.Token);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        if (_table is not null)
            await _table.ReloadServerData();
    }

    private Task ReloadAsync()
        => _table is null ? Task.CompletedTask : _table.ReloadServerData();

    private async Task OpenCreateDialogAsync()
    {
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<NotificationTemplateEditorDialog>(T.Notifications.NewTemplateDialogTitle, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(NotificationTemplateDto template)
    {
        var parameters = new DialogParameters { ["Template"] = template };
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<NotificationTemplateEditorDialog>(T.FormatValue(T.Notifications.EditTemplateTitle, template.Name), parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task ReloadIfSavedAsync(IDialogReference dialog)
    {
        var result = await dialog.Result;
        if (result is { Canceled: false })
            await ReloadAsync();
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

    private static string Truncate(string value)
        => value.Length > 60 ? value[..60] + "…" : value;

    public void Dispose() => _searchCts?.Cancel();
}