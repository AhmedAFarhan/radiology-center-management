using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace RadiologyCenter.Desktop.Services;

public sealed class AppAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly TokenStorage _tokenStorage;
    private Task<AuthenticationState>? _cachedState;

    public AppAuthenticationStateProvider(TokenStorage tokenStorage)
    {
        _tokenStorage = tokenStorage;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        _cachedState ??= BuildStateAsync();
        return _cachedState;
    }

    public Task SignInAsync(AuthTokens tokens)
    {
        _tokenStorage.Save(tokens);
        _cachedState = null;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        return Task.CompletedTask;
    }

    public Task SignOutAsync()
    {
        _tokenStorage.Clear();
        _cachedState = null;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        return Task.CompletedTask;
    }

    public void Refresh()
    {
        _cachedState = null;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private Task<AuthenticationState> BuildStateAsync()
    {
        var tokens = _tokenStorage.GetTokens();
        if (tokens is null || tokens.RefreshTokenExpiresAt <= DateTime.UtcNow)
        {
            _tokenStorage.Clear();
            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        }

        var claims = JwtClaimsParser.Parse(tokens.AccessToken);

        var identity = new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.Name, tokens.Username),
                new Claim(ClaimTypes.Email, claims.GetValueOrDefault("email") ?? string.Empty),
                new Claim(ClaimTypes.GivenName, claims.GetValueOrDefault("firstName") ?? string.Empty),
                new Claim(ClaimTypes.Surname, claims.GetValueOrDefault("lastName") ?? string.Empty),
            },
            authenticationType: "AppSession");

        return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
    }
}
