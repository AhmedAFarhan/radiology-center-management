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

public partial class ChangePasswordDialog : EditorDialogBase
{
[Parameter] public bool Forced { get; set; }

    private readonly ChangePasswordModel _model = new();
    private EditContext _editContext = default!;
    private bool _showCurrent;
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
                await AuthService.ChangePasswordAsync(_model.CurrentPassword, _model.NewPassword);
                Snackbar.Add(T.ChangePassword.Success, Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            },
            () => T.ChangePassword.Unreachable);
    }

    private sealed class ChangePasswordModel
    {
        [Required(ErrorMessage = "Current password is required.")]
        public string CurrentPassword { get; set; } = string.Empty;

        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).{8,}$", ErrorMessage = "Password must contain both letters and digits.")]
        public string NewPassword { get; set; } = string.Empty;

        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
