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

        var identityClaims = new List<Claim>
        {
            new(ClaimTypes.Name, tokens.Username),
            new(ClaimTypes.Email, claims.Get("email") ?? string.Empty),
            new(ClaimTypes.GivenName, claims.Get("firstName") ?? string.Empty),
            new(ClaimTypes.Surname, claims.Get("lastName") ?? string.Empty),
        };

        foreach (var permission in claims.GetAll(AppClaimTypes.Permission))
            identityClaims.Add(new Claim(AppClaimTypes.Permission, permission));

        foreach (var role in claims.GetAll("role"))
            identityClaims.Add(new Claim(ClaimTypes.Role, role));

        if (string.Equals(claims.Get(AppClaimTypes.IsAdmin), "true", StringComparison.OrdinalIgnoreCase))
            identityClaims.Add(new Claim(AppClaimTypes.IsAdmin, "true"));

        var identity = new ClaimsIdentity(identityClaims, authenticationType: "AppSession");

        return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
    }
}
