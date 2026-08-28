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
using RadiologyCenter.Desktop.Features.Notifications.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Features.Notifications.Pages;

public partial class NotificationMessages : ListPageBase<NotificationMessageDto>
{
    private string _channel = string.Empty;
    private string _status = string.Empty;
    private IReadOnlyList<EnumOptionDto> _channelOptions = Array.Empty<EnumOptionDto>();
    private IReadOnlyList<EnumOptionDto> _statusOptions = Array.Empty<EnumOptionDto>();

    protected override string UnreachableMessage => T.Notifications.Unreachable;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        try
        {
            _channelOptions = await EnumOptionsService.GetOptionsAsync("NotificationChannel");
            _statusOptions = await EnumOptionsService.GetOptionsAsync("NotificationStatus");
        }
        catch
        {
            // filter options are non-critical; leave empty
        }
    }

    protected override async Task<PagedResult<NotificationMessageDto>> LoadPageAsync(
        string? search,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken ct)
        => await NotificationService.GetMessagesPagedAsync(
            search,
            sortBy,
            sortDescending,
            page,
            pageSize,
            string.IsNullOrWhiteSpace(_channel) ? null : _channel,
            string.IsNullOrWhiteSpace(_status) ? null : _status,
            ct);

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

    private async Task OpenSendDialogAsync()
    {
        var dialog = await DialogService.ShowAsync<SendNotificationDialog>(T.SendDialog.Title, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenDetailAsync(NotificationMessageDto message)
    {
        var parameters = new DialogParameters { ["Message"] = message };
        await DialogService.ShowAsync<NotificationMessageDetailDialog>(T.MessageDialog.Title, parameters, EditorDialogOptions);
    }

    private static Color ChannelColor(string channel) => channel switch
    {
        "Sms" => Color.Info,
        "Email" => Color.Warning,
        "Push" => Color.Secondary,
        _ => Color.Default,
};
}

