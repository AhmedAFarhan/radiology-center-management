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

public partial class RoleEditorDialog : ComponentBase
{
[Parameter] public RoleDto? Role { get; set; }

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private readonly RoleFormModel _model = new();
    private EditContext _editContext = default!;
    private bool _busy;

    private bool IsEdit => Role is not null;

    protected override void OnInitialized()
    {
        _editContext = new EditContext(_model);

        if (Role is null)
            return;

        _model.Name = Role.Name;
        _model.Description = Role.Description;
    }

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        var message = IsEdit ? T.RoleDialog.Updated : T.RoleDialog.Created;
        if (await SafeExecute.RunAsync(
                () => IsEdit
                    ? IdentityService.UpdateRoleAsync(Role!.Id, new UpdateRoleInput
                    {
                        Name = _model.Name,
                        Description = _model.Description,
                    })
                    : IdentityService.CreateRoleAsync(new CreateRoleInput
                    {
                        Name = _model.Name,
                        Description = _model.Description,
                    }),
                Snackbar,
                () => T.RoleDialog.UnreachableRetry,
                busy => _busy = busy))
        {
            Snackbar.Add(message, Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
    }

    private void CancelAsync()
        => MudDialog.Cancel();

    private sealed class RoleFormModel
    {
        [Required(ErrorMessage = "Role name is required.")]
        [MaxLength(256, ErrorMessage = "Role name must be 256 characters or fewer.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Description must be 500 characters or fewer.")]
        public string? Description { get; set; }
    }
}