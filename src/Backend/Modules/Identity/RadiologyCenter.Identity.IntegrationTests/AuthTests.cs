using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using RadiologyCenter.IntegrationTests.Shared;

namespace Tests;

public class AuthTests : TestBase
{
    private const string BaseUrl = "api/auth";

    public AuthTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var command = new { UserName = "admin", Password = "Admin@12345" };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/login", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>();
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsConflict()
    {
        var command = new { UserName = "admin", Password = "WrongPassword1!" };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/login", command);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>();
        body!.Success.Should().BeFalse();
        body.Error.Should().NotBeNull();
        body.Error!.Code.Should().Be("Identity.InvalidCredentials");
    }

    [Fact]
    public async Task Login_WithNonexistentUser_ReturnsConflict()
    {
        var command = new { UserName = "nonexistent_user_xyz", Password = "AnyPassword1!" };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/login", command);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>();
        body!.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Login_WithEmptyUserName_ReturnsBadRequest()
    {
        var command = new { UserName = "", Password = "Admin@12345" };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/login", command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithEmptyPassword_ReturnsBadRequest()
    {
        var command = new { UserName = "admin", Password = "" };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/login", command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RefreshToken_WithValidToken_ReturnsNewToken()
    {
        var loginResult = await LoginAsAdminAsync();
        var command = new { Token = loginResult.RefreshToken };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/refresh", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>();
        body!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task RefreshToken_WithInvalidToken_ReturnsUnauthorized()
    {
        var command = new { Token = "invalid-refresh-token-abc" };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/refresh", command);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>();
        body!.Success.Should().BeFalse();
    }

    [Fact]
    public async Task RefreshToken_WithEmptyToken_ReturnsBadRequest()
    {
        var command = new { Token = "" };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/refresh", command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Logout_WithValidRefreshToken_ReturnsOk()
    {
        var loginResult = await LoginAsAdminAsync();
        var command = new { RefreshToken = loginResult.RefreshToken };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/logout", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>();
        body!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Logout_WithoutRefreshToken_RevokeAllSessions_ReturnsOk()
    {
        await LoginAsAdminAsync();
        var command = new { };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/logout", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Logout_WithRevokedToken_ReturnsUnauthorized()
    {
        var loginResult = await LoginAsAdminAsync();
        var command = new { RefreshToken = loginResult.RefreshToken };
        await Client.PostAsJsonAsync($"{BaseUrl}/logout", command);
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/logout", command);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ChangePassword_WithCorrectCurrentPassword_ReturnsOk()
    {
        await LoginAsAdminAsync();
        var command = new { CurrentPassword = "Admin@12345", NewPassword = "NewAdmin@12345" };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/change-password", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>();
        body!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task ChangePassword_WithWrongCurrentPassword_ReturnsConflict()
    {
        await LoginAsAdminAsync();
        var command = new { CurrentPassword = "WrongOldPassword1!", NewPassword = "NewAdmin@12345" };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/change-password", command);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>();
        body!.Success.Should().BeFalse();
    }

    [Fact]
    public async Task ChangePassword_WithSamePassword_ReturnsBadRequest()
    {
        await LoginAsAdminAsync();
        var command = new { CurrentPassword = "Admin@12345", NewPassword = "Admin@12345" };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/change-password", command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ChangePassword_WithWeakNewPassword_ReturnsBadRequest()
    {
        await LoginAsAdminAsync();
        var command = new { CurrentPassword = "Admin@12345", NewPassword = "weak" };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/change-password", command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ChangePassword_WithoutAuth_ReturnsUnauthorized()
    {
        var unauthClient = Factory.CreateClient();
        var command = new { CurrentPassword = "Admin@12345", NewPassword = "NewAdmin@12345" };
        var response = await unauthClient.PostAsJsonAsync($"{BaseUrl}/change-password", command);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_ThenRefresh_ThenLogout_FullLifecycle_Works()
    {
        var loginResult = await LoginAsAdminAsync();
        loginResult.AccessToken.Should().NotBeNullOrEmpty();
        loginResult.RefreshToken.Should().NotBeNullOrEmpty();

        var refreshCommand = new { Token = loginResult.RefreshToken };
        var refreshResponse = await Client.PostAsJsonAsync($"{BaseUrl}/refresh", refreshCommand);
        refreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var logoutCommand = new { RefreshToken = loginResult.RefreshToken };
        var logoutResponse = await Client.PostAsJsonAsync($"{BaseUrl}/logout", logoutCommand);
        logoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<TokenResultDto> LoginAsAdminAsync()
    {
        var command = new { UserName = "admin", Password = "Admin@12345" };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/login", command);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<TokenResultDto>>();
        return body!.Data!;
    }

    private sealed class ApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
        public ApiErrorDto? Error { get; set; }
    }

    private sealed class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public ApiErrorDto? Error { get; set; }
    }

    private sealed class ApiErrorDto
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
    }

    private sealed class TokenResultDto
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime RefreshTokenExpiresAt { get; set; }
        public bool MustChangePassword { get; set; }
    }
}
