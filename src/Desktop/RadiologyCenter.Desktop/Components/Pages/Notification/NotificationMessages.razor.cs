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
using Color = MudBlazor.Color;

namespace RadiologyCenter.Desktop.Components.Pages.Notification;

public partial class NotificationMessages : ComponentBase, IDisposable
{
private MudTable<NotificationMessageDto>? _table;
    private string? _search;
    private string _channel = string.Empty;
    private string _status = string.Empty;
    private CancellationTokenSource? _searchCts;
    private string? _loadError;
    private bool _offline;

    private async Task<TableData<NotificationMessageDto>> LoadServerData(TableState state, CancellationToken ct)
    {
        try
        {
            var page = await NotificationService.GetMessagesPagedAsync(
                _search,
                state.SortLabel,
                state.SortDirection == SortDirection.Descending,
                state.Page + 1,
                state.PageSize,
                string.IsNullOrWhiteSpace(_channel) ? null : _channel,
                string.IsNullOrWhiteSpace(_status) ? null : _status,
                ct);

            _loadError = null;
            _offline = false;
            return new TableData<NotificationMessageDto> { Items = page.Items, TotalItems = page.TotalCount };
        }
        catch (OperationCanceledException)
        {
            if (ct.IsCancellationRequested)
                return new TableData<NotificationMessageDto> { Items = Array.Empty<NotificationMessageDto>(), TotalItems = 0 };
            throw;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            _loadError = ex.Message;
            _offline = false;
            return new TableData<NotificationMessageDto> { Items = Array.Empty<NotificationMessageDto>(), TotalItems = 0 };
        }
        catch (Exception)
        {
            Snackbar.Add(T.Notifications.Unreachable, Severity.Error);
            _loadError = T.Notifications.Unreachable;
            _offline = true;
            return new TableData<NotificationMessageDto> { Items = Array.Empty<NotificationMessageDto>(), TotalItems = 0 };
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

    private async Task OnChannelChangedAsync(string value)
    {
        _channel = value;
        await ReloadAsync();
    }

    private async Task OnStatusChangedAsync(string value)
    {
        _status = value;
        await ReloadAsync();
    }

    private Task ReloadAsync()
        => _table is null ? Task.CompletedTask : _table.ReloadServerData();

    private async Task OpenSendDialogAsync()
    {
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<SendNotificationDialog>(T.SendDialog.Title, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenDetailAsync(NotificationMessageDto message)
    {
        var parameters = new DialogParameters { ["Message"] = message };
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        await DialogService.ShowAsync<NotificationMessageDetailDialog>(T.MessageDialog.Title, parameters, options);
    }

    private async Task ReloadIfSavedAsync(IDialogReference dialog)
    {
        var result = await dialog.Result;
        if (result is { Canceled: false })
            await ReloadAsync();
    }

    private static Color ChannelColor(string channel) => channel switch
    {
        "Sms" => Color.Info,
        "Email" => Color.Warning,
        "Push" => Color.Secondary,
        _ => Color.Default,
    };

private static string Truncate(string value)
        => value.Length > 55 ? value[..55] + "…" : value;

    public void Dispose() => _searchCts?.Cancel();
}