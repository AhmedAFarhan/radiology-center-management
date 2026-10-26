using RadiologyCenter.Clinic;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Security;
using RadiologyCenter.Desktop.Services;
using Xunit;

// These tests replace the SQL boundary, while using the actual startup and protected-store code.
[Collection("Session startup configuration")]
public sealed class SessionStartupTests
{
    [Fact]
    public async Task RestartRevalidatesSavedCredentialsBeforeStartupBecomesReady()
    {
        using var setup = new StartupScope();
        var saved = SaveSession();
        var business = new BusinessRuntimeService();
        var tokens = new TokenStorage(() => "clinic");
        var backend = new BackendStatusService(business, tokens);
        var revalidation = new TaskCompletionSource<TokenResult>();
        business.Restore = (session, password) =>
        {
            Assert.Equal(saved, session);
            Assert.Equal("SyntheticPassword12!", password);
            return revalidation.Task;
        };

        var startup = backend.StartAsync();
        Assert.False(backend.IsReady);
        revalidation.SetResult(Restored(saved));
        await startup;
        Assert.True(backend.IsReady);
        Assert.False(backend.IsFailed);
        var restartedStore = new TokenStorage(() => "clinic");
        Assert.Equal("revalidated-access", restartedStore.GetTokens()!.AccessToken);
        Assert.Equal(saved.ExpiresAt, restartedStore.GetTokens()!.ExpiresAt);
        Assert.Equal("SyntheticPassword12!", restartedStore.GetDatabasePassword());
    }

    [Theory]
    [InlineData(401)]
    [InlineData(423)]
    public async Task ChangedRevokedOrLockedCredentialsReturnToSignIn(int status)
    {
        using var setup = new StartupScope();
        SaveSession();
        var business = new BusinessRuntimeService
        {
            Restore = (_, _) => throw new ApiException(status, "Synthetic authentication rejection")
        };
        var tokens = new TokenStorage(() => "clinic");
        var backend = new BackendStatusService(business, tokens);
        await backend.StartAsync();
        Assert.True(backend.IsReady);
        Assert.False(backend.IsFailed);
        Assert.True(business.SignedOut);
        Assert.Null(new TokenStorage(() => "clinic").GetTokens());
    }

    [Fact]
    public async Task ConnectionFailurePreservesTheSessionAndRetryCanRestoreIt()
    {
        using var setup = new StartupScope();
        var saved = SaveSession();
        var business = new BusinessRuntimeService
        {
            Restore = (_, _) => throw new TimeoutException("Synthetic LAN interruption")
        };
        var tokens = new TokenStorage(() => "clinic");
        var backend = new BackendStatusService(business, tokens);
        await backend.StartAsync();
        Assert.False(backend.IsReady);
        Assert.True(backend.IsFailed);
        Assert.False(business.SignedOut);
        Assert.Equal(saved, new TokenStorage(() => "clinic").GetTokens());

        business.Restore = (session, _) => Task.FromResult(Restored(session));
        await backend.RetryAsync();
        Assert.True(backend.IsReady);
        Assert.False(backend.IsFailed);
    }

    [Fact]
    public async Task ANewRequiredPasswordChangeCannotBeBypassedByRestoration()
    {
        using var setup = new StartupScope();
        SaveSession();
        var business = new BusinessRuntimeService
        {
            Restore = (session, _) => Task.FromResult(Restored(session) with { MustChangePassword = true })
        };
        var backend = new BackendStatusService(business, new TokenStorage(() => "clinic"));
        await backend.StartAsync();
        Assert.True(backend.IsReady);
        Assert.True(business.SignedOut);
        Assert.Null(new TokenStorage(() => "clinic").GetTokens());
    }

    [Fact]
    public async Task ExpiredSessionDoesNotAttemptAutomaticAuthentication()
    {
        using var setup = new StartupScope();
        SaveSession(DateTime.UtcNow.AddMinutes(-1));
        var business = new BusinessRuntimeService
        {
            Restore = (_, _) => throw new InvalidOperationException("Must not reconnect an expired session.")
        };
        var backend = new BackendStatusService(business, new TokenStorage(() => "clinic"));
        await backend.StartAsync();
        Assert.True(backend.IsReady);
        Assert.False(backend.IsFailed);
        Assert.Null(new TokenStorage(() => "clinic").GetTokens());
    }

    private static AuthTokens SaveSession(DateTime? expires = null)
    {
        var expiry = expires ?? DateTime.UtcNow.AddHours(4);
        var saved = new AuthTokens("old-access", "saved-refresh", expiry, expiry, "synthetic-user");
        new TokenStorage(() => "clinic").Save(saved, "SyntheticPassword12!");
        return saved;
    }
    private static TokenResult Restored(AuthTokens session) => new("revalidated-access", "new-refresh",
        session.ExpiresAt, session.RefreshTokenExpiresAt, false);

    private sealed class StartupScope : IDisposable
    {
        private readonly string? _previous = Environment.GetEnvironmentVariable("EGCARE_CLIENT_CONFIG_PATH");
        private readonly string _directory = Path.Combine(Path.GetTempPath(), "EGcareSessionTests", Guid.NewGuid().ToString("N"));
        private readonly IDisposable _preferences = Preferences.UseIsolatedStore();
        public StartupScope()
        {
            var path = Path.Combine(_directory, "clinic.json");
            new ClinicConnectionProfile { Server = "SYNTHETIC-SERVER", Database = "SyntheticClinic" }.Write(path);
            Environment.SetEnvironmentVariable("EGCARE_CLIENT_CONFIG_PATH", path);
        }
        public void Dispose()
        {
            Environment.SetEnvironmentVariable("EGCARE_CLIENT_CONFIG_PATH", _previous);
            _preferences.Dispose();
            Directory.Delete(_directory, recursive: true);
        }
    }
}

[CollectionDefinition("Session startup configuration", DisableParallelization = true)]
public sealed class SessionStartupCollection { }

namespace RadiologyCenter.Desktop.Services
{
    public sealed class BusinessRuntimeService
    {
        public Func<AuthTokens, string?, Task<TokenResult>> Restore { get; set; } = (_, _) => throw new InvalidOperationException();
        public bool SignedOut { get; private set; }
        public Task EnsureDevelopmentAsync() => Task.CompletedTask;
        public Task<TokenResult> RestoreSessionAsync(AuthTokens session, string? password) => Restore(session, password);
        public Task SignOutAsync() { SignedOut = true; return Task.CompletedTask; }
    }
    public sealed class ApiException(int statusCode, string message) : Exception(message)
    {
        public int StatusCode { get; } = statusCode;
    }
}
