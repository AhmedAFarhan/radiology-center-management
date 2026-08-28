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

public partial class LoginDialog : ComponentBase
{
[CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private readonly LoginModel _model = new();
    private bool _busy;
    private bool _showPassword;

    private async Task SubmitAsync()
    {
        await SafeExecute.RunAsync(async () =>
            {
                var mustChangePassword = await AuthService.SignInAsync(_model.UserName, _model.Password);
                if (mustChangePassword)
                {
                    Snackbar.Add(T.Login.MustChangePassword, Severity.Warning);
                    MudDialog.Close(DialogResult.Ok(true));
                    var options = new DialogOptions
                    {
                        CloseOnEscapeKey = false,
                        BackdropClick = false,
                        MaxWidth = MaxWidth.Small,
                        FullWidth = true,
                        NoHeader = true,
                    };
                    await DialogService.ShowAsync<ChangePasswordDialog>(T.ChangePassword.Title, new DialogParameters { ["Forced"] = true }, options);
                    Navigation.NavigateTo(ReturnUrl.Consume() ?? "/dashboard");
                    return;
                }

                Snackbar.Add(T.Login.Welcome, Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
                Navigation.NavigateTo(ReturnUrl.Consume() ?? "/dashboard");
            },
            Snackbar,
            () => T.Login.Unreachable,
            busy => _busy = busy);
    }

    private sealed class LoginModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
    }
}
