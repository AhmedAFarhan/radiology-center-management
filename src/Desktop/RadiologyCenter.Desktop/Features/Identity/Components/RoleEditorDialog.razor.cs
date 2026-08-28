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
using RadiologyCenter.Desktop.Features.Identity.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Features.Identity.Components;

public partial class RoleEditorDialog : EditorDialogBase
{
[Parameter] public RoleDto? Role { get; set; }

    private readonly RoleFormModel _model = new();
    private EditContext _editContext = default!;

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
        if (await TrySaveAsync(
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
                () => T.RoleDialog.UnreachableRetry))
        {
            Snackbar.Add(message, Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
    }


}
