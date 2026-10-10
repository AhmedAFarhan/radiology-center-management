using System.Net.Http.Json;
using System.Text.Json;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using RadiologyCenter.Identity.Infrastructure.Persistence;
using RadiologyCenter.Identity.Domain.Entities;

namespace RadiologyCenter.IntegrationTests.Shared;

public abstract class TestBase : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    protected CustomWebApplicationFactory Factory { get; }
    protected HttpClient Client { get; }

    protected TestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        Factory.AssertTestDatabase(context.Database.GetConnectionString());
        var administrator = await context.Users.SingleAsync(u => u.UserName == TestCredentials.UserName);
        administrator.Unlock();
        administrator.SetPasswordHash(new PasswordHasher<User>().HashPassword(administrator, TestCredentials.Password));
        administrator.ClearDomainEvents();
        await context.SaveChangesAsync();
        await LoginAsAdminAsync();
    }

    public Task DisposeAsync() { Client.Dispose(); return Task.CompletedTask; }

    private async Task LoginAsAdminAsync()
    {
        var loginPayload = new { UserName = TestCredentials.UserName, Password = TestCredentials.Password };
        var response = await Client.PostAsJsonAsync("/api/auth/login", loginPayload);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<LoginResponse>(JsonOptions);
        if (string.IsNullOrEmpty(body?.Data?.AccessToken))
            throw new InvalidOperationException("Failed to obtain access token from login endpoint.");

        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", body.Data.AccessToken);
    }

    private sealed class LoginResponse
    {
        public bool Success { get; set; }
        public TokenData? Data { get; set; }
    }

    private sealed class TokenData
    {
        public string? AccessToken { get; set; }
    }
}
