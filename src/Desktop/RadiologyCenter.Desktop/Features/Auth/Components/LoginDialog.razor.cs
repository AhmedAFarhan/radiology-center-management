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
using RadiologyCenter.Desktop.Features.Auth.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Features.Auth.Components;

public partial class LoginDialog : ComponentBase
{
[CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private readonly LoginModel _model = new();
    private bool _busy;
    private bool _showPassword;
    private bool _updateAvailable, _disposed;

    protected override void OnInitialized()
    {
        _updateAvailable = Updates.Available is not null;
        Updates.Changed += OnUpdatesChanged;
    }

    private void OnUpdatesChanged()
    {
        if (!_disposed) _ = InvokeAsync(() => { _updateAvailable = Updates.Available is not null; StateHasChanged(); });
    }

    public void Dispose() { _disposed = true; Updates.Changed -= OnUpdatesChanged; }

    private Task OpenClinicUpdates() => UpdateDialogs.ShowAsync();

    private async Task OpenClinicSettings()
    {
        await DialogService.ShowAsync<RadiologyCenter.Desktop.Shared.Components.ClinicSettingsDialog>("EGcare",new DialogOptions { MaxWidth=MaxWidth.Medium,FullWidth=true });
    }
    private async Task SubmitAsync()
    {
        await SafeExecute.RunAsync(async () =>
            {
                var mustChangePassword = await AuthService.SignInAsync(_model.UserName, _model.Password);
                _model.Password = string.Empty;
                MudDialog.Close(DialogResult.Ok(true));
                await Features.Auth.Services.SignInCompletion.CompleteAsync(
                    mustChangePassword,
                    async () =>
                    {
                        Snackbar.Add(T.Login.MustChangePassword, Severity.Warning);
                        var options = new DialogOptions
                        {
                            CloseOnEscapeKey = false,
                            BackdropClick = false,
                            MaxWidth = MaxWidth.Small,
                            FullWidth = true,
                            NoHeader = true,
                        };
                        var dialog = await DialogService.ShowAsync<ChangePasswordDialog>(
                            T.ChangePassword.Title, new DialogParameters { ["Forced"] = true }, options);
                        // ShowAsync returns when displayed; Result completes after the user finishes.
                        var result = await dialog.Result;
                        return result is { Canceled: false, Data: true };
                    },
                    () => AuthService.SignOutAsync(),
                    () =>
                    {
                        Snackbar.Add(T.Login.Welcome, Severity.Success);
                        Navigation.NavigateTo(ReturnUrl.Consume() ?? "/dashboard");
                    });
            },
            Snackbar,
            () => T.Login.Unreachable,
            busy => _busy = busy);
    }


}
