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

namespace RadiologyCenter.Desktop.Features.Auth.Components;

public partial class ResetPasswordDialog : EditorDialogBase
{
[Parameter] public UserDto User { get; set; } = default!;

    private readonly ResetPasswordModel _model = new();
    private EditContext _editContext = default!;
    private bool _showNew;
    private bool _showConfirm;

    protected override void OnInitialized()
        => _editContext = new EditContext(_model);

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        await TrySaveAsync(async () =>
            {
                await IdentityService.ResetPasswordAsync(User.Id, _model.NewPassword);
                Snackbar.Add(T.FormatValue(T.UserDialog.PasswordReset, User.UserName), Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            },
            () => T.UserDialog.Unreachable);
    }

    private sealed class ResetPasswordModel
    {
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).{8,}$", ErrorMessage = "Password must contain both letters and digits.")]
        public string NewPassword { get; set; } = string.Empty;

        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
