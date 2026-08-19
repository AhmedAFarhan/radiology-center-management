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

public partial class NotificationMessages : ListPageBase<NotificationMessageDto>
{
    private string _channel = string.Empty;
    private string _status = string.Empty;

    protected override string UnreachableMessage => T.Notifications.Unreachable;

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