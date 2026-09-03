using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using RadiologyCenter.IntegrationTests.Shared;

namespace Tests;

public class UsersTests : TestBase
{
    private const string BaseUrl = "api/users";

    public UsersTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetById_ExistingUser_ReturnsOk()
    {
        var userId = await CreateTestUserAsync();
        var response = await Client.GetAsync($"{BaseUrl}/{userId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(userId);
    }

    [Fact]
    public async Task GetById_NonexistentUser_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.GetAsync($"{BaseUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_InvalidGuid_ReturnsBadRequest()
    {
        var response = await Client.GetAsync($"{BaseUrl}/not-a-guid");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAll_ReturnsPagedResult()
    {
        var request = new { PageNumber = 1, PageSize = 10 };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/all", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<UserListItemDto>>>();
        body!.Success.Should().BeTrue();
        body.Data!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAll_WithSearchTerm_FiltersResults()
    {
        var request = new { PageNumber = 1, PageSize = 10, SearchTerm = "admin" };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/all", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<UserListItemDto>>>();
        body!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Create_ValidUser_ReturnsOk()
    {
        var command = new
        {
            UserName = $"testuser_{Guid.NewGuid():N}",
            Email = $"test_{Guid.NewGuid():N}@example.com",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "01012345678",
            Password = "Test@12345",
            RoleIds = new object[] { }
        };
        var response = await Client.PostAsJsonAsync(BaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>();
        body!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Create_DuplicateEmail_ReturnsConflict()
    {
        var email = $"dup_{Guid.NewGuid():N}@example.com";
        var command1 = new
        {
            UserName = $"user1_{Guid.NewGuid():N}",
            Email = email,
            FirstName = "First",
            LastName = "User",
            PhoneNumber = "01012345678",
            Password = "Test@12345",
            RoleIds = new object[] { }
        };
        await Client.PostAsJsonAsync(BaseUrl, command1);

        var command2 = new
        {
            UserName = $"user2_{Guid.NewGuid():N}",
            Email = email,
            FirstName = "Second",
            LastName = "User",
            PhoneNumber = "01098765432",
            Password = "Test@12345",
            RoleIds = new object[] { }
        };
        var response = await Client.PostAsJsonAsync(BaseUrl, command2);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Create_DuplicateUsername_ReturnsConflict()
    {
        var userName = $"dupuser_{Guid.NewGuid():N}";
        var command1 = new
        {
            UserName = userName,
            Email = $"a_{Guid.NewGuid():N}@example.com",
            FirstName = "First",
            LastName = "User",
            PhoneNumber = "01012345678",
            Password = "Test@12345",
            RoleIds = new object[] { }
        };
        await Client.PostAsJsonAsync(BaseUrl, command1);

        var command2 = new
        {
            UserName = userName,
            Email = $"b_{Guid.NewGuid():N}@example.com",
            FirstName = "Second",
            LastName = "User",
            PhoneNumber = "01098765432",
            Password = "Test@12345",
            RoleIds = new object[] { }
        };
        var response = await Client.PostAsJsonAsync(BaseUrl, command2);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Create_MissingRequiredFields_ReturnsBadRequest()
    {
        var command = new { };
        var response = await Client.PostAsJsonAsync(BaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_InvalidEmail_ReturnsBadRequest()
    {
        var command = new
        {
            UserName = $"testuser_{Guid.NewGuid():N}",
            Email = "not-an-email",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "01012345678",
            Password = "Test@12345",
            RoleIds = new object[] { }
        };
        var response = await Client.PostAsJsonAsync(BaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_InvalidPhoneNumber_ReturnsBadRequest()
    {
        var command = new
        {
            UserName = $"testuser_{Guid.NewGuid():N}",
            Email = $"test_{Guid.NewGuid():N}@example.com",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "123",
            Password = "Test@12345",
            RoleIds = new object[] { }
        };
        var response = await Client.PostAsJsonAsync(BaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_WeakPassword_ReturnsBadRequest()
    {
        var command = new
        {
            UserName = $"testuser_{Guid.NewGuid():N}",
            Email = $"test_{Guid.NewGuid():N}@example.com",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "01012345678",
            Password = "weak",
            RoleIds = new object[] { }
        };
        var response = await Client.PostAsJsonAsync(BaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateProfile_ExistingUser_ReturnsOk()
    {
        var userId = await CreateTestUserAsync();
        var command = new { FirstName = "Updated", LastName = "Name", PhoneNumber = "01055555555" };
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{userId}/profile", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateProfile_NonexistentUser_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var command = new { FirstName = "Updated", LastName = "Name", PhoneNumber = (string?)null };
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{fakeId}/profile", command);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateProfile_EmptyFirstName_ReturnsBadRequest()
    {
        var userId = await CreateTestUserAsync();
        var command = new { FirstName = "", LastName = "Name", PhoneNumber = (string?)null };
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{userId}/profile", command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Activate_DeactivatedUser_ReturnsOk()
    {
        var userId = await CreateTestUserAsync();
        await Client.PostAsJsonAsync($"{BaseUrl}/{userId}/deactivate", new { });
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{userId}/activate", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Activate_NonexistentUser_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{fakeId}/activate", new { });
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Deactivate_ExistingUser_ReturnsOk()
    {
        var userId = await CreateTestUserAsync();
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{userId}/deactivate", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Deactivate_NonexistentUser_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{fakeId}/deactivate", new { });
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Lock_ExistingUser_ReturnsOk()
    {
        var userId = await CreateTestUserAsync();
        var lockoutEnd = DateTimeOffset.UtcNow.AddMinutes(30);
        var command = new { LockoutEnd = lockoutEnd };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{userId}/lock", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Lock_NonexistentUser_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var command = new { LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(30) };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{fakeId}/lock", command);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Lock_PastDate_ReturnsBadRequest()
    {
        var userId = await CreateTestUserAsync();
        var command = new { LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(-10) };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{userId}/lock", command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Unlock_LockedUser_ReturnsOk()
    {
        var userId = await CreateTestUserAsync();
        var lockCommand = new { LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(30) };
        await Client.PostAsJsonAsync($"{BaseUrl}/{userId}/lock", lockCommand);
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{userId}/unlock", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Unlock_NonexistentUser_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{fakeId}/unlock", new { });
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ResetPassword_ExistingUser_ReturnsOk()
    {
        var userId = await CreateTestUserAsync();
        var command = new { NewPassword = "Reset@12345" };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{userId}/reset-password", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ResetPassword_NonexistentUser_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var command = new { NewPassword = "Reset@12345" };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{fakeId}/reset-password", command);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ResetPassword_WeakPassword_ReturnsBadRequest()
    {
        var userId = await CreateTestUserAsync();
        var command = new { NewPassword = "weak" };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{userId}/reset-password", command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AssignRole_ExistingUserAndRole_ReturnsOk()
    {
        var userId = await CreateTestUserAsync();
        var roleId = await CreateTestRoleAsync();
        var command = new { RoleId = roleId };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{userId}/roles", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AssignRole_NonexistentUser_ReturnsNotFound()
    {
        var fakeUserId = Guid.NewGuid();
        var roleId = await CreateTestRoleAsync();
        var command = new { RoleId = roleId };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{fakeUserId}/roles", command);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AssignRole_NonexistentRole_ReturnsNotFound()
    {
        var userId = await CreateTestUserAsync();
        var fakeRoleId = Guid.NewGuid();
        var command = new { RoleId = fakeRoleId };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{userId}/roles", command);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RemoveRole_ExistingAssignment_ReturnsOk()
    {
        var userId = await CreateTestUserAsync();
        var roleId = await CreateTestRoleAsync();
        await Client.PostAsJsonAsync($"{BaseUrl}/{userId}/roles", new { RoleId = roleId });
        var response = await Client.DeleteAsync($"{BaseUrl}/{userId}/roles/{roleId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RemoveRole_NonexistentUser_ReturnsNotFound()
    {
        var fakeUserId = Guid.NewGuid();
        var roleId = await CreateTestRoleAsync();
        var response = await Client.DeleteAsync($"{BaseUrl}/{fakeUserId}/roles/{roleId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateRoles_ExistingUser_ReturnsOk()
    {
        var userId = await CreateTestUserAsync();
        var roleId = await CreateTestRoleAsync();
        var command = new { RoleIds = new[] { roleId } };
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{userId}/roles", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateRoles_NonexistentUser_ReturnsNotFound()
    {
        var fakeUserId = Guid.NewGuid();
        var command = new { RoleIds = Array.Empty<Guid>() };
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{fakeUserId}/roles", command);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateRoles_EmptyRoleIds_ReturnsBadRequest()
    {
        var userId = await CreateTestUserAsync();
        var command = new { RoleIds = Array.Empty<Guid>() };
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{userId}/roles", command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private async Task<Guid> CreateTestUserAsync()
    {
        var command = new
        {
            UserName = $"testuser_{Guid.NewGuid():N}",
            Email = $"test_{Guid.NewGuid():N}@example.com",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "01012345678",
            Password = "Test@12345",
            RoleIds = new object[] { }
        };
        var createResponse = await Client.PostAsJsonAsync(BaseUrl, command);
        createResponse.EnsureSuccessStatusCode();

        var allResponse = await Client.PostAsJsonAsync($"{BaseUrl}/all", new { PageNumber = 1, PageSize = 1, SearchTerm = command.UserName });
        allResponse.EnsureSuccessStatusCode();
        var allBody = await allResponse.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<UserListItemDto>>>();
        return allBody!.Data!.Items.First().Id;
    }

    private async Task<Guid> CreateTestRoleAsync()
    {
        var command = new { Name = $"TestRole_{Guid.NewGuid():N}", Description = "Test role" };
        var createResponse = await Client.PostAsJsonAsync("api/roles", command);
        createResponse.EnsureSuccessStatusCode();

        var allResponse = await Client.PostAsJsonAsync("api/roles/all", new { PageNumber = 1, PageSize = 1 });
        allResponse.EnsureSuccessStatusCode();
        var allBody = await allResponse.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<RoleDto>>>();
        return allBody!.Data!.Items.First().Id;
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

    private sealed class UserDto
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }

    private sealed class UserListItemDto
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsActive { get; set; }
    }

    private sealed class RoleDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }

    private sealed class PagedResultDto<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
