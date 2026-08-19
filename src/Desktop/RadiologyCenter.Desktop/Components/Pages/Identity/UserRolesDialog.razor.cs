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

public partial class UserRolesDialog : ComponentBase
{
[Parameter] public UserDto User { get; set; } = default!;

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private List<RoleDto> _roles = new();
    private readonly Dictionary<string, bool> _selected = new();
    private string? _loadError;
    private bool _busy;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var roles = await IdentityService.GetRolesPagedAsync(null, "Name", false, 1, 100);
            _roles = roles.Items.ToList();
            foreach (var role in _roles)
                _selected[role.Id] = false;
        }
        catch (ApiException ex)
        {
            _loadError = ex.Message;
        }
        catch (Exception)
        {
            _loadError = T.UserDialog.LoadRolesError;
        }
    }

    private async Task SubmitAsync()
    {
        var selectedIds = _selected.Where(kv => kv.Value).Select(kv => kv.Key).ToList();
        if (selectedIds.Count == 0)
        {
            Snackbar.Add(T.UserDialog.SelectRole, Severity.Warning);
            return;
        }

        await SafeExecute.RunAsync(async () =>
            {
                await IdentityService.UpdateUserRolesAsync(User.Id, selectedIds);
                Snackbar.Add(T.UserDialog.RolesUpdated, Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            },
            Snackbar,
            () => T.UserDialog.Unreachable,
            busy => _busy = busy);
    }

    private void CancelAsync()
        => MudDialog.Cancel();
}