using System.Text;
using System.Text.Json;
using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Services;

public sealed class AuthService
{
    private readonly ApiClient _api;
    private readonly AppAuthenticationStateProvider _authState;
    private readonly TokenStorage _tokenStorage;

    public AuthService(ApiClient api, AppAuthenticationStateProvider authState, TokenStorage tokenStorage)
    {
        _api = api;
        _authState = authState;
        _tokenStorage = tokenStorage;
    }

    public async Task SignInAsync(string userName, string password, CancellationToken ct = default)
    {
        var tokens = await _api.PostAsync<TokenResult>("api/auth/login", new { userName, password }, ct);

        await _authState.SignInAsync(new AuthTokens(
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.ExpiresAt,
            tokens.RefreshTokenExpiresAt,
            userName));
    }

    public async Task SignOutAsync(CancellationToken ct = default)
    {
        var tokens = _tokenStorage.GetTokens();
        if (tokens is not null)
        {
            try
            {
                await _api.SendAsync("api/auth/logout", new { userId = JwtClaims.GetSubject(tokens.AccessToken), refreshToken = tokens.RefreshToken }, ct);
            }
            catch
            {
                // best-effort server-side revoke
            }
        }

        await _authState.SignOutAsync();
    }
}

internal static class JwtClaims
{
    public static Guid? GetSubject(string accessToken)
    {
        try
        {
            var parts = accessToken.Split('.');
            if (parts.Length < 2)
                return null;

            var payload = parts[1]
                .Replace('-', '+')
                .Replace('_', '/');
            switch (payload.Length % 4)
            {
                case 2: payload += "=="; break;
                case 3: payload += "="; break;
            }

            using var document = JsonDocument.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(payload)));
            if (document.RootElement.TryGetProperty("sub", out var sub) &&
                Guid.TryParse(sub.GetString(), out var userId))
            {
                return userId;
            }
        }
        catch
        {
            // ignore malformed tokens
        }

        return null;
    }
}
