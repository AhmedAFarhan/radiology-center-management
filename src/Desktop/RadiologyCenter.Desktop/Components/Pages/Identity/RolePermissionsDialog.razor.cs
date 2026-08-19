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

namespace RadiologyCenter.Desktop.Components.Pages.Identity;

public partial class RolePermissionsDialog : ComponentBase
{
[Parameter] public RoleDto Role { get; set; } = default!;

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private readonly Dictionary<string, bool> _selected = new();
    private List<string> _groups = new();
    private Dictionary<string, List<PermissionDto>> _byGroup = new();
    private bool _loading = true;
    private string? _loadError;
    private bool _busy;

    protected override async Task OnInitializedAsync()
    {
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        _loading = true;
        _loadError = null;
        try
        {
            var permissions = await IdentityService.GetPermissionsAsync();

            _groups = permissions
                .Select(p => p.Group)
                .Where(g => !string.IsNullOrWhiteSpace(g))
                .Cast<string>()
                .Distinct()
                .OrderBy(g => g, StringComparer.OrdinalIgnoreCase)
                .ToList();

            _byGroup = _groups.ToDictionary(
                g => g,
                g => permissions
                    .Where(p => string.Equals(p.Group, g, StringComparison.Ordinal))
                    .OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
                    .ToList());

            foreach (var permission in permissions)
                _selected.TryAdd(permission.Code, Role.Permissions.Contains(permission.Code, StringComparer.OrdinalIgnoreCase));
        }
        catch (ApiException ex)
        {
            _loadError = ex.Message;
        }
        catch (Exception)
        {
            _loadError = T.RoleDialog.Unreachable;
        }
        finally
        {
            _loading = false;
        }
    }

    private string GroupName(string group)
        => T.Get($"roles.permissiongroup.{group.ToLowerInvariant()}", group);

    private string PermissionName(PermissionDto permission)
        => T.Get($"roles.permission.{permission.Code.Replace(".", "").Replace("-", "")}", permission.Name);

    private async Task SubmitAsync()
    {
        await SafeExecute.RunAsync(async () =>
            {
                var current = Role.Permissions.Select(p => p.ToLowerInvariant()).ToHashSet();
                var next = _selected.Where(kv => kv.Value).Select(kv => kv.Key.ToLowerInvariant()).ToHashSet();

                foreach (var add in next.Except(current))
                    await IdentityService.AddPermissionToRoleAsync(Role.Id, add);

                foreach (var remove in current.Except(next))
                    await IdentityService.RemovePermissionFromRoleAsync(Role.Id, remove);

                Snackbar.Add(T.RoleDialog.PermissionsUpdated, Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            },
            Snackbar,
            () => T.RoleDialog.Unreachable,
            busy => _busy = busy);
    }

    private void CancelAsync()
        => MudDialog.Cancel();
}