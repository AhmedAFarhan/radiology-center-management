namespace RadiologyCenter.Desktop.Security;

using RadiologyCenter.Desktop.Models;

public sealed record AuthTokens(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    DateTime RefreshTokenExpiresAt,
    string Username,
    bool MustChangePassword = false)
{
    public static AuthTokens From(TokenResult result, string username)
        => new(
            result.AccessToken,
            result.RefreshToken,
            result.ExpiresAt,
            result.RefreshTokenExpiresAt,
            username,
            result.MustChangePassword);
}

public sealed class TokenStorage
{
    private const string AccessTokenKey = "auth.access_token";
    private const string RefreshTokenKey = "auth.refresh_token";
    private const string ExpiresAtKey = "auth.expires_at";
    private const string RefreshExpiresAtKey = "auth.refresh_expires_at";
    private const string UsernameKey = "auth.username";
    private const string MustChangePasswordKey = "auth.must_change_password";

    public AuthTokens? GetTokens()
    {
        var accessToken = DpapiProtector.Unprotect(Preferences.Default.Get(AccessTokenKey, string.Empty));
        var refreshToken = DpapiProtector.Unprotect(Preferences.Default.Get(RefreshTokenKey, string.Empty));

        if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(refreshToken))
            return null;

        return new AuthTokens(
            accessToken,
            refreshToken,
            DateTime.Parse(Preferences.Default.Get(ExpiresAtKey, DateTime.MinValue.ToString("o"))),
            DateTime.Parse(Preferences.Default.Get(RefreshExpiresAtKey, DateTime.MinValue.ToString("o"))),
            Preferences.Default.Get(UsernameKey, string.Empty),
            Preferences.Default.Get(MustChangePasswordKey, false));
    }

    public void Save(AuthTokens tokens)
    {
        Preferences.Default.Set(AccessTokenKey, DpapiProtector.Protect(tokens.AccessToken));
        Preferences.Default.Set(RefreshTokenKey, DpapiProtector.Protect(tokens.RefreshToken));
        Preferences.Default.Set(ExpiresAtKey, tokens.ExpiresAt.ToString("o"));
        Preferences.Default.Set(RefreshExpiresAtKey, tokens.RefreshTokenExpiresAt.ToString("o"));
        Preferences.Default.Set(UsernameKey, tokens.Username);
        Preferences.Default.Set(MustChangePasswordKey, tokens.MustChangePassword);
    }

    public void Clear()
    {
        Preferences.Default.Remove(AccessTokenKey);
        Preferences.Default.Remove(RefreshTokenKey);
        Preferences.Default.Remove(ExpiresAtKey);
        Preferences.Default.Remove(RefreshExpiresAtKey);
        Preferences.Default.Remove(UsernameKey);
        Preferences.Default.Remove(MustChangePasswordKey);
    }
}
