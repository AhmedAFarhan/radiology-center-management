using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace RadiologyCenter.IntegrationTests.Shared;

public abstract class TestBase : IClassFixture<CustomWebApplicationFactory>
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
        LoginAsAdminAsync().GetAwaiter().GetResult();
    }

    private async Task LoginAsAdminAsync()
    {
        var loginPayload = new { UserName = "admin123", Password = "admin123" };
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
