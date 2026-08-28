using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace RadiologyCenter.Desktop.Security;

public sealed class PermissionService
{
    private readonly AuthenticationStateProvider _auth;
    private HashSet<string> _permissions = new(StringComparer.Ordinal);
    private bool _isAdmin;
    private bool _ready;

    public PermissionService(AuthenticationStateProvider auth)
    {
        _auth = auth;
        _auth.AuthenticationStateChanged += OnAuthenticationStateChanged;
        _ = RefreshAsync(_auth.GetAuthenticationStateAsync());
    }

    public event Action? ReadyChanged;

    public bool Ready => _ready;

    public bool IsAdmin => _isAdmin;

    public bool HasPermission(string code)
        => _isAdmin || _permissions.Contains(code);

    public bool HasAny(params string[] codes)
        => _isAdmin || codes.Any(_permissions.Contains);

    private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
        => _ = RefreshAsync(task);

    private async Task RefreshAsync(Task<AuthenticationState> task)
    {
        try
        {
            var state = await task;
            _permissions = new HashSet<string>(
                state.User.FindAll(AppClaimTypes.Permission).Select(c => c.Value),
                StringComparer.Ordinal);
            _isAdmin = state.User.HasClaim(AppClaimTypes.IsAdmin, "true");
        }
        catch
        {
            _permissions = new HashSet<string>(StringComparer.Ordinal);
            _isAdmin = false;
        }
        finally
        {
            _ready = true;
            ReadyChanged?.Invoke();
        }
    }
}