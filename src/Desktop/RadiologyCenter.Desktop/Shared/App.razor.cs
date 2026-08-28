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
using RadiologyCenter.Desktop.Shared;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Shared;

public partial class App : ComponentBase
{
private Action? _onBackendChanged;
    private Action? _onLanguageChanged;
    private int _languageKey;

    protected override void OnInitialized()
    {
        _onBackendChanged = () => InvokeAsync(StateHasChanged);
        _onLanguageChanged = () => InvokeAsync(async () =>
        {
            await SyncHtmlAsync();
            _languageKey++;
            StateHasChanged();
        });
        Backend.StateChanged += _onBackendChanged;
        T.LanguageChanged += _onLanguageChanged;
        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
            await SyncHtmlAsync();
    }

    private Task SyncHtmlAsync()
        => JS.InvokeVoidAsync("setAppLanguage", T.CurrentCulture, T.IsRTL ? "rtl" : "ltr").AsTask();

    public void Dispose()
    {
        if (_onBackendChanged is not null)
            Backend.StateChanged -= _onBackendChanged;
        if (_onLanguageChanged is not null)
            T.LanguageChanged -= _onLanguageChanged;
    }
}