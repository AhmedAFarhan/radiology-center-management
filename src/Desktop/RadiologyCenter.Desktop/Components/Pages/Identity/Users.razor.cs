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
using RadiologyCenter.Desktop.Components.Pages.Auth;

namespace RadiologyCenter.Desktop.Components.Pages.Identity;

public partial class Users : ComponentBase, IDisposable
{
private MudTable<UserDto>? _table;
    private string? _search;
    private string? _currentUserName;
    private CancellationTokenSource? _searchCts;
    private string? _loadError;
    private bool _offline;
    private string? _openId;

    [SupplyParameterFromQuery(Name = "q")]
    public string? SearchQuery { get; set; }

    [SupplyParameterFromQuery(Name = "open")]
    public string? OpenId { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (!string.IsNullOrWhiteSpace(OpenId) && Guid.TryParse(OpenId, out _))
            _openId = OpenId;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_openId is not null)
        {
            var id = _openId;
            _openId = null;
            await OpenByDeepLinkAsync(id);
        }
    }

    private async Task OpenByDeepLinkAsync(string id)
    {
        UserDto? user = null;
        var ok = await SafeExecute.RunAsync(
            async () => user = await IdentityService.GetUserByIdAsync(id),
            Snackbar,
            () => T.Users.Unreachable);

        if (ok && user is not null)
        {
            var parameters = new DialogParameters { ["User"] = user };
            var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
            var dialog = await DialogService.ShowAsync<UserEditorDialog>(T.FormatValue(T.UserDialog.EditTitle, user.UserName), parameters, options);
            await ReloadIfSavedAsync(dialog);
        }

        NavigationManager.NavigateTo("/users", replace: true);
    }

    protected override void OnInitialized()
        => _currentUserName = TokenStorage.GetTokens()?.Username;

    private bool IsCurrentUser(UserDto user)
        => string.Equals(user.UserName, _currentUserName, StringComparison.OrdinalIgnoreCase);

    private async Task<TableData<UserDto>> LoadServerData(TableState state, CancellationToken ct)
    {
        try
        {
            var page = await IdentityService.GetUsersPagedAsync(
                _search,
                state.SortLabel,
                state.SortDirection == SortDirection.Descending,
                state.Page + 1,
                state.PageSize,
                ct);

            _loadError = null;
            _offline = false;
            return new TableData<UserDto> { Items = page.Items, TotalItems = page.TotalCount };
        }
        catch (OperationCanceledException)
        {
            if (ct.IsCancellationRequested)
                return new TableData<UserDto> { Items = Array.Empty<UserDto>(), TotalItems = 0 };
            throw;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            _loadError = ex.Message;
            _offline = false;
            return new TableData<UserDto> { Items = Array.Empty<UserDto>(), TotalItems = 0 };
        }
        catch (Exception)
        {
            Snackbar.Add(T.Users.Unreachable, Severity.Error);
            _loadError = T.Users.Unreachable;
            _offline = true;
            return new TableData<UserDto> { Items = Array.Empty<UserDto>(), TotalItems = 0 };
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
        var dialog = await DialogService.ShowAsync<UserEditorDialog>(T.UserDialog.NewTitle, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(UserDto user)
    {
        var parameters = new DialogParameters { ["User"] = user };
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<UserEditorDialog>(T.FormatValue(T.UserDialog.EditTitle, user.UserName), parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenRolesDialogAsync(UserDto user)
    {
        var parameters = new DialogParameters { ["User"] = user };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<UserRolesDialog>(T.FormatValue(T.UserDialog.RolesTitle, user.UserName), parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenLockDialogAsync(UserDto user)
    {
        var parameters = new DialogParameters { ["User"] = user };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<UserLockDialog>(T.FormatValue(T.UserDialog.LockTitle, user.UserName), parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenResetPasswordDialogAsync(UserDto user)
    {
        var parameters = new DialogParameters { ["User"] = user };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<ResetPasswordDialog>(T.FormatValue(T.UserDialog.ResetPasswordTitle, user.UserName), parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task ReloadIfSavedAsync(IDialogReference dialog)
    {
        var result = await dialog.Result;
        if (result is { Canceled: false })
            await ReloadAsync();
    }

    private async Task ActivateAsync(UserDto user)
    {
        await SafeExecute.RunAsync(async () =>
            {
                await IdentityService.ActivateUserAsync(user.Id);
                Snackbar.Add(T.FormatValue(T.Users.Activated, user.UserName), Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Users.Unreachable);
    }

    private async Task DeactivateAsync(UserDto user)
    {
        var confirmed = await DialogService.ShowMessageBoxAsync(
            T.Users.DeactivateTitle,
            T.FormatValue(T.Users.DeactivateConfirm, user.UserName),
            T.Users.Deactivate,
            T.Common.Cancel);

        if (confirmed != true)
            return;

        await SafeExecute.RunAsync(async () =>
            {
                await IdentityService.DeactivateUserAsync(user.Id);
                Snackbar.Add(T.FormatValue(T.Users.Deactivated, user.UserName), Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Users.Unreachable);
    }

    private async Task UnlockAsync(UserDto user)
    {
        await SafeExecute.RunAsync(async () =>
            {
                await IdentityService.UnlockUserAsync(user.Id);
                Snackbar.Add(T.FormatValue(T.Users.Unlocked, user.UserName), Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Users.Unreachable);
    }

    private string StatusText(UserDto user)
    {
        if (user.IsLocked)
            return T.Users.Locked;
        return user.IsActive ? T.Common.Active : T.Common.Inactive;
    }

    public void Dispose() => _searchCts?.Cancel();
}