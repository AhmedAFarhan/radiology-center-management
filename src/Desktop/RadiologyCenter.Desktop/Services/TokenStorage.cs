namespace RadiologyCenter.Desktop.Services;

public sealed record AuthTokens(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    DateTime RefreshTokenExpiresAt,
    string Username,
    bool MustChangePassword = false);

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
        var accessToken = Preferences.Default.Get(AccessTokenKey, string.Empty);
        var refreshToken = Preferences.Default.Get(RefreshTokenKey, string.Empty);

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
        Preferences.Default.Set(AccessTokenKey, tokens.AccessToken);
        Preferences.Default.Set(RefreshTokenKey, tokens.RefreshToken);
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
