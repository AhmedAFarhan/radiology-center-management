using System.Collections.Concurrent;
using RadiologyCenter.Desktop.Security;
using RadiologyCenter.Desktop.Services;
using Xunit;

public sealed class SessionPersistenceTests
{
    [Fact]
    public void RestartRestoresTheOriginalProtectedSessionAndItsDeadline()
    {
        var context = Guid.NewGuid().ToString();
        var original = Tokens();
        using var preferences = Preferences.UseIsolatedStore();
        var firstProcess = new TokenStorage(() => context);
        firstProcess.Save(original, "SyntheticPassword12!");

        var saved = Assert.Single(Preferences.Default.Values).Value;
        Assert.StartsWith("dpapi:", saved);
        Assert.DoesNotContain("SyntheticPassword12!", saved);
        Assert.DoesNotContain(original.AccessToken, saved);
        var restartedProcess = new TokenStorage(() => context);
        Assert.Equal(original, restartedProcess.GetTokens());
        Assert.Equal("SyntheticPassword12!", restartedProcess.GetDatabasePassword());

        // Reissued UI tokens must keep the same SQL credential and fixed session deadline.
        restartedProcess.Save(original with { AccessToken = "new-access-token" });
        var nextRestart = new TokenStorage(() => context);
        Assert.Equal(original.RefreshTokenExpiresAt, nextRestart.GetTokens()!.RefreshTokenExpiresAt);
        Assert.Equal("SyntheticPassword12!", nextRestart.GetDatabasePassword());
    }

    [Fact]
    public void LogoutRemovesCredentialsAndTokensForTheNextProcess()
    {
        using var preferences = Preferences.UseIsolatedStore();
        var storage = new TokenStorage(() => "clinic");
        storage.Save(Tokens(), "SyntheticPassword12!");
        storage.Clear();
        Assert.Null(storage.GetTokens());
        Assert.Null(new TokenStorage(() => "clinic").GetTokens());
        Assert.Empty(Preferences.Default.Values);
    }

    [Fact]
    public void ExpiredOrDifferentClinicSessionsCannotBeRestored()
    {
        using var preferences = Preferences.UseIsolatedStore();
        var storage = new TokenStorage(() => "clinic-one");
        storage.Save(Tokens() with { RefreshTokenExpiresAt = DateTime.UtcNow.AddMinutes(-1) }, "SyntheticPassword12!");
        Assert.Null(new TokenStorage(() => "clinic-one").GetTokens());
        Assert.Empty(Preferences.Default.Values);

        storage.Save(Tokens(), "SyntheticPassword12!");
        Assert.Null(new TokenStorage(() => "clinic-two").GetTokens());
        Assert.Empty(Preferences.Default.Values);
    }

    [Fact]
    public void RequiredPasswordChangeSessionOnlyLivesInTheCurrentProcess()
    {
        using var preferences = Preferences.UseIsolatedStore();
        var storage = new TokenStorage(() => "clinic");
        storage.Save(Tokens() with { MustChangePassword = true }, "InitialPassword12!");
        Assert.True(storage.GetTokens()!.MustChangePassword);
        Assert.Empty(Preferences.Default.Values);
        Assert.Null(new TokenStorage(() => "clinic").GetTokens());

        storage.Save(Tokens(), "ChangedPassword12!");
        Assert.Equal("ChangedPassword12!", new TokenStorage(() => "clinic").GetDatabasePassword());
    }

    [Fact]
    public void SigningInAsAnotherUserDoesNotInheritThePreviousPassword()
    {
        using var preferences = Preferences.UseIsolatedStore();
        var storage = new TokenStorage(() => "clinic");
        storage.Save(Tokens(), "FirstPassword12!");
        storage.Save(Tokens() with { Username = "second-user" });
        Assert.Null(new TokenStorage(() => "clinic").GetDatabasePassword());
    }

    [Theory]
    [InlineData("plain:plaintext-secret")]
    [InlineData("dpapi:corrupt")]
    [InlineData("unprotected-secret")]
    public void UnprotectedOrCorruptSessionsAreRejectedAndRemoved(string value)
    {
        using var preferences = Preferences.UseIsolatedStore();
        Preferences.Default.Set("auth.protected_session.v1", value);
        Assert.Null(new TokenStorage(() => "clinic").GetTokens());
        Assert.Empty(Preferences.Default.Values);
        Assert.Empty(DpapiProtector.Unprotect(value));
    }

    [Fact]
    public async Task ConcurrentReadsDoNotLoseTheSavedSessionDuringDecryption()
    {
        using var preferences = Preferences.UseIsolatedStore();
        var original = Tokens();
        new TokenStorage(() => "clinic").Save(original, "SyntheticPassword12!");
        var restartedProcess = new TokenStorage(() => "clinic");
        var reads = await Task.WhenAll(Enumerable.Range(0, 24).Select(_ => Task.Run(restartedProcess.GetTokens)));
        Assert.All(reads, restored => Assert.Equal(original, restored));
        Assert.Single(Preferences.Default.Values);
    }

    [Theory]
    [InlineData("{\"Tokens\":null,\"Context\":\"clinic\"}")]
    [InlineData("not-json")]
    public void InvalidProtectedPayloadDoesNotPreventStartup(string payload)
    {
        using var preferences = Preferences.UseIsolatedStore();
        Preferences.Default.Set("auth.protected_session.v1", DpapiProtector.Protect(payload));
        Assert.Null(new TokenStorage(() => "clinic").GetTokens());
        Assert.Empty(Preferences.Default.Values);
    }

    private static AuthTokens Tokens() => new("synthetic-access-token", "synthetic-refresh-token",
        DateTime.UtcNow.AddHours(4), DateTime.UtcNow.AddHours(4), "synthetic-user");
}

// Replace only MAUI preference I/O; tests execute the real Windows DPAPI and session store.
namespace RadiologyCenter.Desktop.Security
{
    internal sealed class Preferences
    {
        private static readonly AsyncLocal<Preferences?> Current = new();
        public static Preferences Default => Current.Value ?? throw new InvalidOperationException("Use an isolated test store.");
        public ConcurrentDictionary<string, string> Values { get; } = new();
        public string Get(string key, string fallback) => Values.TryGetValue(key, out var value) ? value : fallback;
        public void Set(string key, string value) => Values[key] = value;
        public void Remove(string key) => Values.TryRemove(key, out _);
        public static IDisposable UseIsolatedStore()
        {
            var previous = Current.Value;
            Current.Value = new Preferences();
            return new Restore(() => Current.Value = previous);
        }
        private sealed class Restore(Action restore) : IDisposable { public void Dispose() => restore(); }
    }
}
