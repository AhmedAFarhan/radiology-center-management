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

namespace RadiologyCenter.Desktop.Features.Identity.Pages;

public partial class Roles : ListPageBase<RoleDto>
{
    protected override string UnreachableMessage => T.Roles.Unreachable;

    protected override async Task<PagedResult<RoleDto>> LoadPageAsync(
        string? search,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken ct)
        => await IdentityService.GetRolesPagedAsync(search, sortBy, sortDescending, page, pageSize, ct);

    private async Task OpenCreateDialogAsync()
    {
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<RoleEditorDialog>(T.RoleDialog.NewRole, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(RoleDto role)
    {
        var parameters = new DialogParameters { ["Role"] = role };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<RoleEditorDialog>(T.FormatValue(T.Roles.EditTitle, role.Name), parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenPermissionsDialogAsync(RoleDto role)
    {
        var parameters = new DialogParameters { ["Role"] = role };
        var options = new DialogOptions { MaxWidth = MaxWidth.Large, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<RolePermissionsDialog>(T.FormatValue(T.RoleDialog.PermissionsTitle, role.Name), parameters, options);
        await ReloadIfSavedAsync(dialog);
    }
}
