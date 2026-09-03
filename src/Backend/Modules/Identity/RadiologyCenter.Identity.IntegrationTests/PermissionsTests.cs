using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using RadiologyCenter.IntegrationTests.Shared;

namespace Tests;

public class PermissionsTests : TestBase
{
    private const string PermissionsUrl = "api/permissions";

    public PermissionsTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetAll_ReturnsPermissionsList()
    {
        var response = await Client.GetAsync(PermissionsUrl);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<PermissionDto>>>();
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAll_PermissionsHaveCodeAndName()
    {
        var response = await Client.GetAsync(PermissionsUrl);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<PermissionDto>>>();
        foreach (var permission in body!.Data!)
        {
            permission.Code.Should().NotBeNullOrEmpty();
            permission.Name.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public async Task GetAll_ContainsExpectedPermissionCodes()
    {
        var response = await Client.GetAsync(PermissionsUrl);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<PermissionDto>>>();
        var codes = body!.Data!.Select(p => p.Code).ToList();
        codes.Should().Contain("users.read");
        codes.Should().Contain("users.create");
        codes.Should().Contain("roles.read");
    }

    [Fact]
    public async Task GetAll_PermissionsHaveGroup()
    {
        var response = await Client.GetAsync(PermissionsUrl);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<PermissionDto>>>();
        body!.Data!.Should().OnlyContain(p => p.Group != null);
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

    private sealed class PermissionDto
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Group { get; set; }
    }
}
