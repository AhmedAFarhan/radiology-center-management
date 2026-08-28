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

public partial class UserLockDialog : ComponentBase
{
[Parameter] public UserDto User { get; set; } = default!;

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private readonly UserLockModel _model = new()
    {
        LockUntilDate = DateTime.Today.AddDays(1),
        LockUntilTime = TimeSpan.FromHours(8),
    };
    private EditContext _editContext = default!;
    private bool _busy;

    protected override void OnInitialized()
        => _editContext = new EditContext(_model);

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        if (_model.LockUntilDate is null || _model.LockUntilTime is null)
            return;

        var lockoutEnd = _model.LockUntilDate.Value.Date + _model.LockUntilTime.Value;
        if (lockoutEnd <= DateTime.Now)
        {
            Snackbar.Add(T.UserDialog.LockFuture, Severity.Warning);
            return;
        }

        await SafeExecute.RunAsync(async () =>
            {
                await IdentityService.LockUserAsync(User.Id, lockoutEnd);
                Snackbar.Add(T.FormatValue(T.UserDialog.LockedUntil, User.UserName, lockoutEnd), Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            },
            Snackbar,
            () => T.UserDialog.Unreachable,
            busy => _busy = busy);
    }

    private void CancelAsync()
        => MudDialog.Cancel();


}
