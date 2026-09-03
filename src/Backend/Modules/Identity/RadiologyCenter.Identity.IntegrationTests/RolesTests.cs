using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using RadiologyCenter.IntegrationTests.Shared;

namespace Tests;

public class RolesTests : TestBase
{
    private const string RolesUrl = "api/roles";

    public RolesTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetById_ExistingRole_ReturnsOk()
    {
        var roleId = await CreateTestRoleAsync();
        var response = await Client.GetAsync($"{RolesUrl}/{roleId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<RoleDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(roleId);
    }

    [Fact]
    public async Task GetById_NonexistentRole_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.GetAsync($"{RolesUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_InvalidGuid_ReturnsBadRequest()
    {
        var response = await Client.GetAsync($"{RolesUrl}/not-a-guid");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAll_ReturnsPagedResult()
    {
        var request = new { PageNumber = 1, PageSize = 10 };
        var response = await Client.PostAsJsonAsync($"{RolesUrl}/all", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<RoleDto>>>();
        body!.Success.Should().BeTrue();
        body.Data!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Create_ValidRole_ReturnsOk()
    {
        var command = new { Name = $"TestRole_{Guid.NewGuid():N}", Description = "A test role" };
        var response = await Client.PostAsJsonAsync(RolesUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>();
        body!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Create_DuplicateName_ReturnsConflict()
    {
        var name = $"DupRole_{Guid.NewGuid():N}";
        await Client.PostAsJsonAsync(RolesUrl, new { Name = name, Description = (string?)null });
        var response = await Client.PostAsJsonAsync(RolesUrl, new { Name = name, Description = (string?)null });
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Create_EmptyName_ReturnsBadRequest()
    {
        var command = new { Name = "", Description = (string?)null };
        var response = await Client.PostAsJsonAsync(RolesUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_NameTooLong_ReturnsBadRequest()
    {
        var command = new { Name = new string('A', 101), Description = (string?)null };
        var response = await Client.PostAsJsonAsync(RolesUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Update_ExistingRole_ReturnsOk()
    {
        var roleId = await CreateTestRoleAsync();
        var command = new { RoleId = roleId, Name = $"Updated_{Guid.NewGuid():N}", Description = "Updated desc" };
        var response = await Client.PutAsJsonAsync($"{RolesUrl}/{roleId}", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Update_NonexistentRole_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var command = new { RoleId = fakeId, Name = "Updated", Description = (string?)null };
        var response = await Client.PutAsJsonAsync($"{RolesUrl}/{fakeId}", command);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_DuplicateName_ReturnsConflict()
    {
        var name1 = $"Role1_{Guid.NewGuid():N}";
        var name2 = $"Role2_{Guid.NewGuid():N}";
        await Client.PostAsJsonAsync(RolesUrl, new { Name = name1, Description = (string?)null });
        var role2Response = await Client.PostAsJsonAsync(RolesUrl, new { Name = name2, Description = (string?)null });
        role2Response.StatusCode.Should().Be(HttpStatusCode.OK);
        var allResponse = await Client.PostAsJsonAsync($"{RolesUrl}/all", new { PageNumber = 1, PageSize = 100, SearchTerm = name2 });
        allResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var allBody = await allResponse.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<RoleDto>>>();
        var roleId2 = allBody!.Data!.Items.Single(r => r.Name == name2).Id;

        var command = new { RoleId = roleId2, Name = name1, Description = (string?)null };
        var response = await Client.PutAsJsonAsync($"{RolesUrl}/{roleId2}", command);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Update_SameName_NoConflict()
    {
        var name = $"SameName_{Guid.NewGuid():N}";
        var createResponse = await Client.PostAsJsonAsync(RolesUrl, new { Name = name, Description = "desc" });
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var allResponse = await Client.PostAsJsonAsync($"{RolesUrl}/all", new { PageNumber = 1, PageSize = 100, SearchTerm = name });
        allResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var allBody = await allResponse.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<RoleDto>>>();
        var roleId = allBody!.Data!.Items.Single(r => r.Name == name).Id;

        var command = new { RoleId = roleId, Name = name, Description = "updated" };
        var response = await Client.PutAsJsonAsync($"{RolesUrl}/{roleId}", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AddPermission_ValidRoleAndPermission_ReturnsOk()
    {
        var roleId = await CreateTestRoleAsync();
        var command = new { PermissionCode = "users.read" };
        var response = await Client.PostAsJsonAsync($"{RolesUrl}/{roleId}/permissions", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AddPermission_NonexistentRole_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var command = new { PermissionCode = "users.read" };
        var response = await Client.PostAsJsonAsync($"{RolesUrl}/{fakeId}/permissions", command);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddPermission_InvalidPermissionCode_ReturnsNotFound()
    {
        var roleId = await CreateTestRoleAsync();
        var command = new { PermissionCode = "nonexistent.permission" };
        var response = await Client.PostAsJsonAsync($"{RolesUrl}/{roleId}/permissions", command);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddPermission_EmptyCode_ReturnsBadRequest()
    {
        var roleId = await CreateTestRoleAsync();
        var command = new { PermissionCode = "" };
        var response = await Client.PostAsJsonAsync($"{RolesUrl}/{roleId}/permissions", command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RemovePermission_ExistingPermission_ReturnsOk()
    {
        var roleId = await CreateTestRoleAsync();
        await Client.PostAsJsonAsync($"{RolesUrl}/{roleId}/permissions", new { PermissionCode = "users.read" });
        var response = await Client.DeleteAsync($"{RolesUrl}/{roleId}/permissions/users.read");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RemovePermission_NonexistentRole_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.DeleteAsync($"{RolesUrl}/{fakeId}/permissions/users.read");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RemovePermission_InvalidPermissionCode_ReturnsNotFound()
    {
        var roleId = await CreateTestRoleAsync();
        var response = await Client.DeleteAsync($"{RolesUrl}/{roleId}/permissions/nonexistent.permission");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddPermission_DuplicatePermission_NoError()
    {
        var roleId = await CreateTestRoleAsync();
        await Client.PostAsJsonAsync($"{RolesUrl}/{roleId}/permissions", new { PermissionCode = "users.read" });
        var response = await Client.PostAsJsonAsync($"{RolesUrl}/{roleId}/permissions", new { PermissionCode = "users.read" });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<Guid> CreateTestRoleAsync()
    {
        var command = new { Name = $"TestRole_{Guid.NewGuid():N}", Description = "Test role" };
        var createResponse = await Client.PostAsJsonAsync(RolesUrl, command);
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var allResponse = await Client.PostAsJsonAsync($"{RolesUrl}/all", new { PageNumber = 1, PageSize = 10, SearchTerm = command.Name });
        allResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var allBody = await allResponse.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<RoleDto>>>();
        return allBody!.Data!.Items.Single(r => r.Name == command.Name).Id;
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

    private sealed class RoleDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsSystem { get; set; }
        public bool IsActive { get; set; }
        public List<string> Permissions { get; set; } = new();
    }

    private sealed class PagedResultDto<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
