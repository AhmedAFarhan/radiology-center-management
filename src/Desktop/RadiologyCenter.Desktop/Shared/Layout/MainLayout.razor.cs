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
using RadiologyCenter.Desktop.Services;
using RadiologyCenter.Desktop.Features.Auth.Components;
using FocusEventArgs = Microsoft.AspNetCore.Components.Web.FocusEventArgs;

namespace RadiologyCenter.Desktop.Shared.Layout;

public partial class MainLayout : LayoutComponentBase, IDisposable
{
[CascadingParameter]
    private Task<AuthenticationState>? AuthStateTask { get; set; }

private bool _shouldRender = true;
private bool _drawerOpen = true;
    private bool _loggingOut;
    private string _userName = string.Empty;
    private string _displayName = string.Empty;
    private string _userEmail = string.Empty;
    private string _userFullName = string.Empty;

    private enum SidebarMode { Wide, Compact, Overlay }

    private const int CompactBreakpoint = 1200;
    private const int OverlayBreakpoint = 800;

    private SidebarMode _sidebarMode = SidebarMode.Wide;
    private bool _userOpen = true;
    private DotNetObjectReference<MainLayout>? _jsRef;

    private string SidebarModeClass => _sidebarMode switch
    {
        SidebarMode.Compact => "nav-compact",
        SidebarMode.Overlay => "nav-overlay",
        _ => string.Empty,
    };

    private string SidebarClass => _sidebarMode == SidebarMode.Overlay
        ? (_drawerOpen ? "open" : "closed")
        : (_drawerOpen ? "open" : "collapsed");

    private bool ShowSidebarScrim => _sidebarMode == SidebarMode.Overlay && _drawerOpen;

    protected override bool ShouldRender()
    {
        var render = _shouldRender;
        _shouldRender = false;
        return render;
    }

    private void ToggleSidebar()
    {
        _drawerOpen = !_drawerOpen;
        if (_sidebarMode == SidebarMode.Wide)
            _userOpen = _drawerOpen;
        _shouldRender = true;
        StateHasChanged();
    }

    private void CloseSidebarOverlay()
    {
        if (_sidebarMode == SidebarMode.Overlay)
        {
            _drawerOpen = false;
            _shouldRender = true;
            StateHasChanged();
        }
    }

    [JSInvokable]
    public void OnWindowResized(int width)
    {
        var mode = width >= CompactBreakpoint ? SidebarMode.Wide
            : width >= OverlayBreakpoint ? SidebarMode.Compact
            : SidebarMode.Overlay;

        if (mode == _sidebarMode)
            return;

        if (_sidebarMode == SidebarMode.Wide)
            _userOpen = _drawerOpen;

        _sidebarMode = mode;

        if (mode == SidebarMode.Wide)
            _drawerOpen = _userOpen;
        else if (mode == SidebarMode.Compact)
            _drawerOpen = false;

        _shouldRender = true;
        StateHasChanged();
    }

    private GlobalSearch? _globalSearch;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (AuthStateTask is not null)
            _ = RefreshUserAsync(AuthStateTask);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _shouldRender = true;
        BusyState.Instance.Changed += OnBusyChanged;
        Connection.Start();
        Navigation.LocationChanged += OnLocationChanged;
        SearchSvc.UserName = _userName;
        SearchSvc.OnStateChanged += HandleSearchStateChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _jsRef = DotNetObjectReference.Create(this);
            await JS.InvokeVoidAsync("trackSidebarResize", _jsRef);
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        if (_sidebarMode == SidebarMode.Overlay && _drawerOpen)
        {
            _drawerOpen = false;
            _shouldRender = true;
            InvokeAsync(StateHasChanged);
        }
    }

    private void OnBusyChanged() => InvokeAsync(StateHasChanged);

    private Task HandleSearchStateChanged()
    {
        _shouldRender = true;
        return InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        Navigation.LocationChanged -= OnLocationChanged;
        BusyState.Instance.Changed -= OnBusyChanged;
        SearchSvc.OnStateChanged -= HandleSearchStateChanged;
        SearchSvc.Dispose();
        _ = JS.InvokeVoidAsync("untrackSidebarResize");
        _jsRef?.Dispose();
    }

private async Task RefreshUserAsync(Task<AuthenticationState> task)
    {
        var state = await task;
        var user = state.User;
        _userName = user.Identity?.Name ?? string.Empty;
        _displayName = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
        if (string.IsNullOrWhiteSpace(_displayName))
            _displayName = _userName;

        _userEmail = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? string.Empty;
        var lastName = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value ?? string.Empty;
        _userFullName = string.Join(' ', new[] { _displayName, lastName }.Where(p => !string.IsNullOrWhiteSpace(p)));
        SearchSvc.UserName = _userName;
        _shouldRender = true;
    }

private async Task OnLogout()
    {
        var parameters = new DialogParameters
        {
            ["Title"] = T.Layout.SignOut,
            ["Message"] = T.Layout.SignOutConfirm,
            ["Icon"] = Icons.Material.Filled.Logout,
            ["Color"] = Color.Primary,
            ["ConfirmText"] = T.Layout.SignOut,
            ["CancelText"] = T.Common.Cancel,
        };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false })
        {
            _loggingOut = true;
            _shouldRender = true;
            StateHasChanged();

            try
            {
                await AuthService.SignOutAsync();
            }
            finally
            {
                Snackbar.Add(T.Layout.SignedOut, Severity.Info);
                Navigation.NavigateTo("/");
            }
        }
    }

    private async Task OnChangePassword()
    {
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader = true };
        await DialogService.ShowAsync<ChangePasswordDialog>(T.ChangePassword.Title, options);
    }

    private void OnNotifications()
        => Snackbar.Add(T.Layout.NoNotifications, Severity.Info);

private string GetInitial()
    {
        if (string.IsNullOrWhiteSpace(_displayName))
            return "?";
        return _displayName[..1].ToUpperInvariant();
    }

    // ── Keyboard / layout shortcuts ──

    private async Task HandleGlobalKeyDown(KeyboardEventArgs e)
    {
        if ((e.CtrlKey || e.MetaKey) && string.Equals(e.Key, "k", StringComparison.OrdinalIgnoreCase))
        {
            await FocusSearchAsync();
        }
    }

    private async Task FocusSearchAsync()
    {
        if (_globalSearch is not null)
            await _globalSearch.FocusAsync();

        if (string.IsNullOrWhiteSpace(SearchSvc.SearchText))
            SearchSvc.ShowRecentSearches();
    }

    private void OnSearchBoxClick() => SearchSvc.OnSearchBoxClick();

    private async Task OnSearchTextChanged(string? value) => await SearchSvc.OnSearchTextChanged(value);

    private async Task OnSearchKeyDown(KeyboardEventArgs e) => await SearchSvc.OnSearchKeyDown(e);

    private void OnSearchBlur(FocusEventArgs e) => SearchSvc.OnSearchBlur(e);

    private void CloseSearch() => SearchSvc.CloseSearch();

    private async Task RecentClickedAsync(string term) => await SearchSvc.OnRecentClicked(term);

    private async Task RetrySearchAsync() => await SearchSvc.RetrySearchAsync();

    private async Task OnActivateItem(SearchFlatItem item) => await SearchSvc.OnActivateItem(item);

    private async Task OnViewAll(string entityType) => await SearchSvc.OnViewAll(entityType);
}
