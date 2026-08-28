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
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;
using RadiologyCenter.Desktop.Features.Auth.Components;

namespace RadiologyCenter.Desktop.Features.Identity.Pages;

public partial class Users : ListPageBase<UserDto>
{
    private string? _currentUserName;

    protected override string BaseRoute => "/users";

    protected override string UnreachableMessage => T.Users.Unreachable;

    protected override async Task<PagedResult<UserDto>> LoadPageAsync(
        string? search,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken ct)
        => await IdentityService.GetUsersPagedAsync(search, sortBy, sortDescending, page, pageSize, ct);

    protected override async Task OpenByDeepLinkAsync(string id)
    {
        UserDto? user = null;
        var ok = await SafeExecute.RunAsync(
            async () => { user = await IdentityService.GetUserByIdAsync(id); },
            Snackbar,
            () => T.Users.Unreachable);

        if (ok && user is not null)
        {
            var parameters = new DialogParameters { ["User"] = user };
            var dialog = await DialogService.ShowAsync<UserEditorDialog>(T.FormatValue(T.UserDialog.EditTitle, user.UserName), parameters, EditorDialogOptions);
            await ReloadIfSavedAsync(dialog);
        }

        NavigationManager.NavigateTo(BaseRoute, replace: true);
    }

    protected override void OnInitialized()
        => _currentUserName = TokenStorage.GetTokens()?.Username;

    private bool IsCurrentUser(UserDto user)
        => string.Equals(user.UserName, _currentUserName, StringComparison.OrdinalIgnoreCase);

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

    private async Task ActivateAsync(UserDto user)
    {
        if (!await ConfirmDialogs.ConfirmStatusChangeAsync(DialogService, T, T.Users.Activate, user.UserName, activating: true))
            return;

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
        var parameters = new DialogParameters
        {
            ["Title"] = T.Users.DeactivateTitle,
            ["Message"] = T.FormatValue(T.Users.DeactivateConfirm, user.UserName),
            ["Icon"] = Icons.Material.Filled.Block,
            ["Color"] = MudBlazor.Color.Warning,
            ["ConfirmText"] = T.Users.Deactivate,
            ["CancelText"] = T.Common.Cancel,
        };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, options);
        var result = await dialog.Result;

        if (result?.Canceled != false)
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
}

