using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace RadiologyCenter.Desktop.Services;

/// <summary>
/// Provides the current user's timezone ID from the authentication state.
/// Caches the timezone after first access for synchronous use in Razor templates.
/// </summary>
public sealed class UserTimezoneService
{
    private readonly AuthenticationStateProvider _authStateProvider;
    private string? _cachedTimeZoneId;

    public UserTimezoneService(AuthenticationStateProvider authStateProvider)
    {
        _authStateProvider = authStateProvider;
        _authStateProvider.AuthenticationStateChanged += _ => _cachedTimeZoneId = null;
    }

    public string GetTimeZoneId()
    {
        return _cachedTimeZoneId ??= GetTimeZoneIdInternal();
    }

    private string GetTimeZoneIdInternal()
    {
        try
        {
            var state = _authStateProvider.GetAuthenticationStateAsync().Result;
            return state.User.FindFirst("timezone")?.Value ?? "Africa/Cairo";
        }
        catch
        {
            return "Africa/Cairo";
        }
    }
}
