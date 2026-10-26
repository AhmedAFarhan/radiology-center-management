namespace RadiologyCenter.Desktop.Security;

using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;

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
    private const string SessionKey = "auth.protected_session.v1";
    private readonly Func<string> _context;
    private readonly object _gate = new();
    private SavedSession? _current;
    private bool _loaded;

    private sealed record SavedSession(AuthTokens Tokens, string Context, string? DatabasePassword);

    public TokenStorage() : this(CurrentContext) { }
    internal TokenStorage(Func<string> context) => _context = context;

    private const string AccessTokenKey = "auth.access_token";
    private const string RefreshTokenKey = "auth.refresh_token";
    private const string ExpiresAtKey = "auth.expires_at";
    private const string RefreshExpiresAtKey = "auth.refresh_expires_at";
    private const string UsernameKey = "auth.username";
    private const string MustChangePasswordKey = "auth.must_change_password";

    public AuthTokens? GetTokens()
    {
        lock (_gate) return ReadSession()?.Tokens;
    }

    internal string? GetDatabasePassword()
    {
        lock (_gate) return ReadSession()?.DatabasePassword;
    }

    public void Save(AuthTokens tokens, string? databasePassword = null)
    {
        lock (_gate)
        {
            var previous = ReadSession();
            var password = databasePassword ?? (previous?.Tokens.Username == tokens.Username ? previous.DatabasePassword : null);
            var session = new SavedSession(tokens, _context(), password);
            // Store one encrypted envelope: credentials, clinic identity and expiry cannot get out of sync.
            // An initial password must be changed before a session can survive an app restart.
            if (tokens.MustChangePassword)
                Preferences.Default.Remove(SessionKey);
            else
                Preferences.Default.Set(SessionKey, DpapiProtector.Protect(JsonSerializer.Serialize(session)));
            ClearLegacyValues();
            _current = session;
            _loaded = true;
        }
    }

    public void Clear()
    {
        lock (_gate)
        {
            _current = null;
            _loaded = true;
            Preferences.Default.Remove(SessionKey);
            ClearLegacyValues();
        }
    }

    private SavedSession? ReadSession()
    {
        if (!_loaded)
        {
            _loaded = true;
            var protectedSession = Preferences.Default.Get(SessionKey, string.Empty);
            try
            {
                var json = DpapiProtector.Unprotect(protectedSession);
                if (!string.IsNullOrEmpty(json)) _current = JsonSerializer.Deserialize<SavedSession>(json);
            }
            catch (JsonException) { }
            if (_current?.Tokens?.MustChangePassword == true) _current = null;
        }
        if (_current is not { Tokens: { } tokens } || _current.Context != _context()
            || string.IsNullOrWhiteSpace(tokens.Username) || string.IsNullOrWhiteSpace(tokens.AccessToken)
            || string.IsNullOrWhiteSpace(tokens.RefreshToken) || tokens.RefreshTokenExpiresAt <= DateTime.UtcNow)
        {
            Clear();
            return null;
        }
        return _current;
    }

    private static string CurrentContext()
    {
        var profile = DesktopConfiguration.Profile;
        var target = DesktopConfiguration.IsDevelopment
            ? "development:" + Path.GetFullPath(AppContext.BaseDirectory)
            : "clinic:" + profile?.Server + "\n" + profile?.Database;
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(target.ToUpperInvariant())));
    }

    private static void ClearLegacyValues()
    {
        Preferences.Default.Remove(AccessTokenKey);
        Preferences.Default.Remove(RefreshTokenKey);
        Preferences.Default.Remove(ExpiresAtKey);
        Preferences.Default.Remove(RefreshExpiresAtKey);
        Preferences.Default.Remove(UsernameKey);
        Preferences.Default.Remove(MustChangePasswordKey);
    }
}
